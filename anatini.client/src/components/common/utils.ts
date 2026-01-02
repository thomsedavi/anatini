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
