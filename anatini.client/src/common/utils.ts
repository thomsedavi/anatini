import type { Content, ContentType, EventOccurrence, Note, Work } from "./types";
import { computed, h, type VNode } from "vue";
import { formatTime } from "./dateUtils";

export function pascalToKebab(string: string) {
  return string.replace(/([a-z])([A-Z])/g, "$1-$2").toLowerCase();
}

export function pascalToCamel(string: string) {
  return string.replace(/^./, match => match.toLowerCase());
}

export function serializeToString(node: Node): string {
  const serializer = new XMLSerializer();
  const xml = serializer.serializeToString(node);
  return xml.replace(" xmlns=\"http://www.w3.org/1999/xhtml\"", "");
}

export function parseFromString(string: string): Node | null {
  const parser = new DOMParser();
  const newDoc = parser.parseFromString(string, 'text/html');
  return newDoc.body.firstChild;
}

// input: html article, e.g. "<article><p>My <em>text</em></p><p>With a...<br>...line <strong>break</strong></p></article>"
// output: article markdown, e.g. "My *article*\n\nWith a...\n...line **break**"
export function parseFromBodyString(string: string | null): string {
  let result = '';

  if (string === null) {
    return result;
  }

  const node = parseFromString(string);

  if (node?.nodeName === 'ARTICLE') {
    node.childNodes.forEach((value, index) => {
      result += paragraphToMarkdown(value as Element);

      if (index < node.childNodes.length - 1) {
        result += '\n\n';
      }
    });
  }

  return result;
}

// input: html element, e.g. "<p>Test<br>Is <strong>strong</strong><p>""
// output: markdown, e.g. "Test\nIs **strong**"
export function paragraphToMarkdown(paragraph: Element): string {
  let result = '';

  paragraph.childNodes.forEach(childNode => {
    if (childNode.nodeType === Node.TEXT_NODE) {
      const text = childNode as Text;

      result += text.textContent;
    } else if (childNode.nodeType === Node.ELEMENT_NODE) {
      const element = childNode as Element;

      if (element.tagName === 'STRONG') {
        result += `**${paragraphToMarkdown(element)}**`;
      } else if (element.tagName === 'EM') {
        result += `*${paragraphToMarkdown(element)}*`;
      } else if (element.tagName === 'BR') {
        result += '\n';
      } else {
        result += element.textContent;
      }
    }
  });

  return result;
}

export function paragraphToHTML(paragraph: Element): string {
  let result = '';

  paragraph.childNodes.forEach(childNode => {
    if (childNode.nodeType === Node.TEXT_NODE) {
      const text = childNode as Text;

      result += text.textContent;
    } else if (childNode.nodeType === Node.ELEMENT_NODE) {
      const element = childNode as Element;

      if (element.tagName === 'STRONG') {
        result += `<strong>${paragraphToHTML(element)}</strong>`;
      } else if (element.tagName === 'EM') {
        result += `<em>${paragraphToHTML(element)}</em>`;
      } else {
        result += element.textContent;
      }
    }
  });

  return result;
}

// input: single line with markdown, e.g. "I am writing *em* and **strong**"
// output: single line with html, e.g. "I am writing <em>em</em> and <strong>strong</strong>"
export function markdownToHtml(line: string): string {
  const cleanedLine = tidy(line);

  const replacementTags = [
    { asteriskCount: 3, openingTags: '<em><strong>', closingTags: '</strong></em>' },
    { asteriskCount: 2, openingTags: '<strong>', closingTags: '</strong>' },
    { asteriskCount: 1, openingTags: '<em>', closingTags: '</em>' }
  ];

  return replaceAsterisks(cleanedLine, replacementTags);
}

// input: single line with markdown, e.g. "I am writing *em* and **strong**", along with replacement tags e.g. {1 (asterisks), <em>, </em>}
// output: single line with html replaced for specific asterisk count, e.g. "I am writing <em>em</em> and **strong**"
function replaceAsterisks(text: string, replacementTags: {asteriskCount: number, openingTags: string, closingTags: string}[]): string {
  const replacementTag = replacementTags[0];

  if (replacementTag === undefined) {
    return text;
  }

  const regExp = new RegExp(`${'\\\*'.repeat(replacementTag.asteriskCount)}(?!\\\*)`, 'g');

  let execArrays = [...text.matchAll(regExp)];

  if (execArrays.length % 2 === 1) {
    execArrays = execArrays.slice(0, execArrays.length - 1);
  }

  const cleanedLineSegments: string[] = [text.substring(0, execArrays[0]?.index ?? text.length)];

  execArrays.forEach((execArray, index) => {
    cleanedLineSegments.push(text.substring(execArray.index + replacementTag.asteriskCount, execArrays[index + 1]?.index ?? text.length));
  });

  let result: string = '';

  cleanedLineSegments.forEach((lineSegment, index) => {
    if (index % 2 === 0) {
      result += lineSegment;
    } else {
      result += `${replacementTag.openingTags}${lineSegment.trim()}${replacementTag.closingTags}`;
    }
  });

  return replaceAsterisks(result, replacementTags.slice(1));
}

// input: multi line string, e.g. "Line one\nLine two\n\nLine three with *em*"
// output: multi line formatted html, e.g. "<p>Line one<br>Line two</p><p>Line three with <em>em</em></p>"
export function formatParagraphs(elementContent: string): string {
  let result = '';

  const lines = elementContent.split('\n');

  const paragraphs: string[][] = [];

  let paragraph: string[] = [];

  lines.forEach(line => {
    let cleanedLine = tidy(line);

    if (cleanedLine.length > 0) {
      const regex = /\[([^\]]+)\]\(([^)]+)\)/g;

      cleanedLine = cleanedLine.replace(regex, (_match: string, linkText, linkUrl) => {
        return `<a href=${linkUrl} target="_blank">${linkText}</a>`;
      });

      const replacementTags = [
        { asteriskCount: 3, openingTags: '<em><strong>', closingTags: '</strong></em>' },
        { asteriskCount: 2, openingTags: '<strong>', closingTags: '</strong>' },
        { asteriskCount: 1, openingTags: '<em>', closingTags: '</em>' }
      ];

      paragraph.push(replaceAsterisks(cleanedLine, replacementTags));
    } else {
      if (paragraph.length > 0) {
        paragraphs.push(paragraph);
      }

      paragraph = [];
    }
  });

  if (paragraph.length > 0) {
    paragraphs.push(paragraph);
  }

  paragraphs.forEach(paragraphLines => {
    result += '<p>';

    paragraphLines.forEach((paragraphLine, index) => {
      result += paragraphLine;

      if (index !== paragraphLines.length - 1) {
        result += '<br>';
      }
    })

    result += '</p>';
  });

  return result;
}

export function getTabIndex(key: string, index: number, length: number): number | undefined {
  if (key === 'ArrowRight') {
    return (index + 1) % length;
  } else if (key === 'ArrowLeft') {
    return (index - 1 + length) % length;
  } else if (key === 'Home') {
    return 0;
  } else if (key === 'End') {
    return length - 1;
  } else {
    return;
  }
}

type Param = string | string[] | undefined;

export type Params = Param[];

export function parseParams(params: Params): string[] {
  const paramsResult: string[] = [];

  params.forEach((param: Param) => {
    if (typeof param === 'string') {
      paramsResult.push(param);
    }
  });

  return paramsResult;
}

/*
Input
```
  
  My   
    messy    
  
    
    whitespace  
  
```

Output
```
My
messy

Text
```
*/
export function tidy(text: string): string {
  const lines = text.split('\n');

  let tidiedLines: string[] = [];

  let breakCounter = 0;

  lines.forEach(line => {
    const tidiedLine = line.trim().replace(/\s+/g, ' ');

    if (tidiedLine === '') {
      breakCounter++;

      if (breakCounter === 1) {
        tidiedLines.push('');
      }
    } else {
      tidiedLines.push(tidiedLine);
      breakCounter = 0;
    }
  });

  while (tidiedLines[0] === '') {
    tidiedLines = tidiedLines.slice(1);
  }

  while (tidiedLines[tidiedLines.length - 1] === '') {
    tidiedLines = tidiedLines.slice(0, tidiedLines.length - 1);
  }

  return tidiedLines.join('\n');
}

export function getContentStatus(content: Content): string {
  if (content.status === 'Draft') {
    return 'Draft';
  } else if (content.status === 'Archived') {
    return 'Archived';
  } else if (content.isScheduled) {
    return 'Scheduled';
  }

  return 'published';
}

export function getContentAddress(content: Content, contentType: ContentType, basis: 'name' | 'id'): string {
  const contentTypeKebab = pascalToKebab(contentType);
  const contentKey = basis === 'id' ? content.id : content.handle;

  if (content.ownerHeader !== null) {
    const ownerTypeKebab = pascalToKebab(content.ownerHeader.type);
    const ownerKey = basis === 'id' ? content.ownerHeader.id : content.ownerHeader.handle;

    return `${ownerTypeKebab}s/${ownerKey}/${contentTypeKebab}s/${contentKey}`;
  } else {
    // return id based address by default
    return `${contentTypeKebab}s/${content.id}`;
  }
}

function parseHtmlToVNodes(input: string): VNode[] {
  if (typeof document === 'undefined') return [];

  const template = document.createElement('template');
  template.innerHTML = input.trim();

  return Array.from(template.content.childNodes)
    .filter((node): node is HTMLElement => node instanceof HTMLElement)
    .map((el) =>
      h(el.tagName.toLowerCase(), { innerHTML: el.innerHTML })
    );
}

export function ContentBody(props: {html: string}) {
  return computed(() => parseHtmlToVNodes(props.html)).value;
}

export function updateContent(content: Content, newContent: Content): void {
  content.isLiked = newContent.isLiked;
  content.isBookmarked = newContent.isBookmarked;
  content.isHidden = newContent.isHidden;
  content.ownerHeader = newContent.ownerHeader;
}

export function isNote(content: Content, contentType: ContentType): content is Note {
  return contentType === 'Note';
}

export function isWork(content: Content, contentType: ContentType): content is Work {
  return contentType === 'Work';
}

export function isEventOccurrence(content: Content, contentType: ContentType): content is EventOccurrence {
  return contentType === 'EventOccurrence';
}

export function getWorkDateTime(publishedAtNz: string): string {
  const publishedAtNzTime = formatTime(publishedAtNz);

  if (publishedAtNzTime === '12:00 am') {
    return `<time datetime="${publishedAtNz.slice(0, 10)}">`;
  } else {
    return `<time datetime="${publishedAtNz}">`;
  }
}
