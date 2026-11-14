import type { Router } from "vue-router";
import type { EventOccurrence, Post, Work } from "./types";

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

export function handleClick(mouseEvent: MouseEvent, router: Router, buttonAction?: (label: string, pressed: string | null) => void): void {
  const anchor = (mouseEvent.target as HTMLElement).closest('a');

  if (anchor !== null) {
    const href = anchor.getAttribute('href');

    if (href !== null) {
      router.push(href);
    }
  }

  const button = (mouseEvent.target as HTMLElement).closest('button');

  if (button !== null) {
    const ariaLabel = button.getAttribute('aria-label');
    const ariaPressed = button.getAttribute('aria-pressed');

    if (ariaLabel !== null) {
      buttonAction?.(ariaLabel, ariaPressed);
    }
  }
}

export function escapeHtml(value: string | null): string | null {
  if (value === null) {
    return null;
  }

  return value.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
}

export function getPostAddress(post: Post, basis: 'name' | 'id'): string {
  const postKey = basis === 'id' ? post.id : post.handle;

  if (post.spaceHeader !== null) {
    const spaceKey = basis === 'id' ? post.spaceHeader.id : post.spaceHeader.handle;

    return `spaces/${spaceKey}/posts/${postKey}`;
  } else if (post.userHeader !== null) {
    const userKey = basis === 'id' ? post.userHeader.id : post.userHeader.handle;

    return `users/${userKey}/posts/${postKey}`;
  } else {
    // return id based address by default
    return `posts/${post.id}`;
  }
}

export function getWorkAddress(work: Work, basis: 'name' | 'id'): string {
  const workKey = basis === 'id' ? work.id : work.handle;

  if (work.spaceHeader !== null) {
    const spaceKey = basis === 'id' ? work.spaceHeader.id : work.spaceHeader.handle;

    return `spaces/${spaceKey}/works/${workKey}`;
  } else if (work.userHeader !== null) {
    const userKey = basis === 'id' ? work.userHeader.id : work.userHeader.handle;

    return `users/${userKey}/works/${workKey}`;
  } else {
    // return id based address by default
    return `works/${work.id}`;
  }
}

export function getEventOccurrenceAddress(eventOccurrence: EventOccurrence, basis: 'name' | 'id'): string {
  const eventKey = basis === 'id' ? eventOccurrence.eventHeader.id : eventOccurrence.eventHeader.handle;
  const eventOccurrenceKey = basis === 'id' ? eventOccurrence.id : eventOccurrence.handle;

  if (eventOccurrence.spaceHeader !== null) {
    const spaceKey = basis === 'id' ? eventOccurrence.spaceHeader.id : eventOccurrence.spaceHeader.handle;

    return `spaces/${spaceKey}/events/${eventKey}/occurrences/${eventOccurrenceKey}`;
  } else if (eventOccurrence.userHeader !== null) {
    const userKey = basis === 'id' ? eventOccurrence.userHeader.id : eventOccurrence.userHeader.handle;

    return `users/${userKey}/events/${eventKey}/occurrences/${eventOccurrenceKey}`;
  } else {
    // return id based address by default
    return `events/${eventOccurrence.eventHeader.id}/occurrence/${eventOccurrence.id}`;
  }
}
