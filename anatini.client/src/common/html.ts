import { apiFetchAuthenticated } from "./apiFetch";
import { formatLong } from "./dateUtils";
import { store } from "./store";
import type { Content, Post, StatusActions, Work } from "./types";

function getFooterHtml(content: Content): string {
  let result = '';

  result += '<footer><menu>'

  if (content.userHeader !== null && store.userId !== null && content.userHeader.id === store.userId) {
    result += `<li><a href='/users/${content.userHeader.handle}/posts/${content.handle}/edit'>Edit</a></li>`
  } else if (store.isAuthenticated) {
    result += `<li><button type='button' aria-label='Dismiss' aria-pressed='${content.hasDismissed ? 'true' : 'false'}'>${content.hasDismissed ? '<svg width="1em" height="1em" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24" /><line x1="1" y1="1" x2="23" y2="23" /></svg>' : '<svg width="1em" height="1em" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z" /><circle cx="12" cy="12" r="3" /></svg>'}</button></li>`;
    result += `<li><button type='button' aria-label='Star' aria-pressed='${content.hasStarred ? 'true' : 'false'}'>${content.hasStarred ? '<svg width="1em" height="1em" viewBox="0 0 24 24" fill="currentColor" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"/></svg>' : '<svg width="1em" height="1em" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"/></svg>'}</button></li>`;
    result += `<li><button type='button' aria-label='Bookmark' aria-pressed='${content.hasBookmarked ? 'true' : 'false'}'>${content.hasBookmarked ? '<svg width="1em" height="1em" viewBox="0 0 24 24" fill="currentColor" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><path d="M19 21l-7-5-7 5V5a2 2 0 0 1 2-2h10a2 2 0 0 1 2 2z" /></svg>' : '<svg width="1em" height="1em" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><path d="M19 21l-7-5-7 5V5a2 2 0 0 1 2-2h10a2 2 0 0 1 2 2z" /></svg>'}</button></li>`;
  }

  result += '</menu></footer>'

  return result;
}

export function getPostHtml(post: Post): string {
  let result = '';

  result += `<header><time datetime='${post.publishedAtNz}'>${formatLong(post.publishedAtNz)}</time></header>`;

  result += post.article;

 result += getFooterHtml(post);

  return result;
}

export function getWorkHtml(work: Work): string {
  let result = '';

  result += '<header>';

  if (work.userHeader !== null) {
    result += `<h3><a href="/users/${work.userHeader.handle}"/work/${work.handle}>${work.name}</a></h3>`;
  } else if (work.spaceHeader !== null) {
    result += `<h3><a href="/spaces/${work.spaceHeader.handle}"/work/${work.handle}>${work.name}</a></h3>`;
  }

  result += '<address role="presentation">';

  if (work.userHeader !== null) {
    result += `<a rel="author" href="/users/${work.userHeader.handle}">`;
    if (work.userHeader.iconImage !== null) {
      result += `<img src="${work.userHeader.iconImage.uri}" alt="">`;
    }
    result += work.userHeader.name;
    result += `</a>`;
  } else if (work.spaceHeader !== null) {
    result += `<a rel="author" href="/spaces/${work.spaceHeader.handle}">`;
    if (work.spaceHeader.iconImage !== null) {
      result += `<img src="${work.spaceHeader.iconImage.uri}" alt="">`;
    }
    result += work.spaceHeader.name;
    result += `</a>`;
  }

  result += '</address>';

  if (work.publishedAtNz !== null) {
    result += `<p>Published: <time ${work.publishedAtNz}>${formatLong(work.publishedAtNz)}</time></p>`;
  }

  result += '</header>';

  result += work.article;

  result += getFooterHtml(work);

  return result;
}

export function buttonAction(label: string, pressed: string | null, content: Content): void {
    const action = label.toLowerCase();

    if (pressed === 'true') {
      const statusActions: StatusActions = {
        204: () => {
          if (action === "bookmark") {
            content.hasBookmarked = null;
          } else if (action === "dismiss") {
            content.hasDismissed = null;
          } else if (action === "star") {
            content.hasStarred = null;
          }
        }
      }

      const init: RequestInit = { method: "DELETE" };

      if (content.spaceHeader !== null) {
        apiFetchAuthenticated({ input: `spaces/${content.spaceHeader.handle}/posts/${content.handle}/${action}`, statusActions, init });
      } else if (content.userHeader !== null) {
        apiFetchAuthenticated({ input: `users/${content.userHeader.handle}/posts/${content.handle}/${action}`, statusActions, init });
      }
    } else {
      const statusActions: StatusActions = {
        201: () => {
          if (action === "bookmark") {
            content.hasBookmarked = true;
          } else if (action === "dismiss") {
            content.hasDismissed = true;
          } else if (action === "star") {
            content.hasStarred = true;
          }
        }
      }

      const init: RequestInit = { method: "POST" };

      if (content.spaceHeader !== null) {
        apiFetchAuthenticated({ input: `spaces/${content.spaceHeader.handle}/posts/${content.handle}/${action}`, statusActions, init });
      } else if (content.userHeader !== null) {
        apiFetchAuthenticated({ input: `users/${content.userHeader.handle}/posts/${content.handle}/${action}`, statusActions, init });
      }
    }
  }
  