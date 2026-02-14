export function serializeToString(node: Node): string {
  const serializer = new XMLSerializer();
  const xml = serializer.serializeToString(node);
  return xml.replace(" xmlns=\"http://www.w3.org/1999/xhtml\"", "");
}

export function parseFromString(string: string): ChildNode | null {
  const parser = new DOMParser();
  const newDoc = parser.parseFromString(string, 'text/html');
  return newDoc.body.firstChild;
}

export function paragraphToMarkdown(paragraph: Element): string {
  let result = '';

  paragraph.childNodes.forEach(childNode => {
    if (childNode.nodeType === Node.TEXT_NODE) {
      const text = childNode as Text;

      result += text.textContent;
    } else if (childNode.nodeType === Node.ELEMENT_NODE) {
      const element = childNode as Element;

      if (element.tagName === 'STRONG') {
        result += `**${element.textContent}**`;
      } else if (element.tagName === 'EM') {
        result += `*${element.textContent}*`;
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
        result += `<strong>${element.textContent}</strong>`;
      } else if (element.tagName === 'EM') {
        result += `<em>${element.textContent}</em>`;
      } else {
        result += element.textContent;
      }
    }
  });

  return result;
}

export function markdownToHtml(line: string): string {
  const cleanedLine = tidy(line);

  const replacementTags = [
    { asteriskCount: 3, openingTags: '<em><strong>', closingTags: '</strong></em>' },
    { asteriskCount: 2, openingTags: '<strong>', closingTags: '</strong>' },
    { asteriskCount: 1, openingTags: '<em>', closingTags: '</em>' }
  ];

  return replaceAsterisks(cleanedLine, replacementTags);
}

function replaceAsterisks(text: string, replacementTags: {asteriskCount: number, openingTags: string, closingTags: string}[]): string {
  if (replacementTags.length === 0) {
    return text;
  }

  const replacementTag = replacementTags[0];

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

export function formatParagraph(elementContent: string): string {
  let result = '';

  const lines = elementContent.split('\n');

  const paragraphs: string[][] = [];

  let paragraph: string[] = [];

  lines.forEach(line => {
    const cleanedLine = tidy(line);

    if (cleanedLine.length > 0) {
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

  paragraphs.push(paragraph);

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

type Param = () => string | string[];

export type Source = Param[];

export function parseSource(source: Source): string[] {
  const params: string[] = [];

  source.forEach((param: Param) => {
    params.push(typeof param === 'string' ? param : param()[0]);
  });

  return params;
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
