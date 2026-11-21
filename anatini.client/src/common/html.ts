import { apiFetchAuthenticated } from "./apiFetch";
import { formatDate, formatDateTime, formatTime, getDaysDifference } from "./dateUtils";
import { store } from "./store";
import type { Content, EventOccurrence, Note, Space, StatusActions, User, Work } from "./types";
import { escapeHtml } from "./utils";

function getOwnerHtml(content: Content): string {
  let result = '';

  result += '<address role="presentation">';

  if (content.userHeader !== null) {
    result += `<a rel="author" href="/users/${content.userHeader.handle}">`;
    if (content.userHeader.iconImage !== null) {
      result += `<img src="${content.userHeader.iconImage.uri}" alt="">`;
    }
    result += content.userHeader.name;
    result += `</a>`;
  } else if (content.spaceHeader !== null) {
    result += `<a rel="author" href="/spaces/${content.spaceHeader.handle}">`;
    if (content.spaceHeader.iconImage !== null) {
      result += `<img src="${content.spaceHeader.iconImage.uri}" alt="">`;
    }
    result += content.spaceHeader.name;
    result += `</a>`;
  }

  result += '</address>';

  return result;
}

function getFooterHtml(content: Content, contentType: string): string {
  let result = '';

  if (store.account.isAuthenticated) {
    result += `<footer><menu role='toolbar' aria-label='Actions'>`

    result += `<li><button type='button' aria-label='Dismiss' aria-pressed='${content.isDismissed ? 'true' : 'false'}'>${content.isDismissed ? '<svg width="1em" height="1em" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24" /><line x1="1" y1="1" x2="23" y2="23" /></svg>' : '<svg width="1em" height="1em" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z" /><circle cx="12" cy="12" r="3" /></svg>'}</button></li>`;
    result += `<li><button type='button' aria-label='Star' aria-pressed='${content.isStarred ? 'true' : 'false'}'>${content.isStarred ? '<svg width="1em" height="1em" viewBox="0 0 24 24" fill="currentColor" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"/></svg>' : '<svg width="1em" height="1em" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"/></svg>'}</button></li>`;
    result += `<li><button type='button' aria-label='Bookmark' aria-pressed='${content.isBookmarked ? 'true' : 'false'}'>${content.isBookmarked ? '<svg width="1em" height="1em" viewBox="0 0 24 24" fill="currentColor" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><path d="M19 21l-7-5-7 5V5a2 2 0 0 1 2-2h10a2 2 0 0 1 2 2z" /></svg>' : '<svg width="1em" height="1em" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><path d="M19 21l-7-5-7 5V5a2 2 0 0 1 2-2h10a2 2 0 0 1 2 2z" /></svg>'}</button></li>`;


    if (content.isOwned) {
      if (content.userHeader !== null) {
        result += `<li><a href='/users/${content.userHeader.handle}/${contentType}/${content.handle}/edit' aria-label='Edit'><svg width="1em" height="1em" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><path d="M17 3a2.828 2.828 0 1 1 4 4L7.5 20.5 2 22l1.5-5.5L17 3z"></path></svg></a></li>`
      } else if (content.spaceHeader !== null) {
        result += `<li><a href='/spaces/${content.spaceHeader.handle}/${contentType}/${content.handle}/edit' aria-label='Edit'><svg width="1em" height="1em" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><path d="M17 3a2.828 2.828 0 1 1 4 4L7.5 20.5 2 22l1.5-5.5L17 3z"></path></svg></a></li>`
      }
    }

    result += '</menu></footer>'
  }

  return result;
}

export function getSpaceHtml(space: Space): string {
  let result = '';

  result += `<header><h3><a href="/spaces/${space.handle}">${space.name}</a></h3></header>`;

  if (space.about !== null) {
    result += space.about
  }

  return result;
}

export function getEventOccurrenceHtml(eventOccurrence: EventOccurrence): string {
  let result = '';

  result += `<header><h4>${escapeHtml(eventOccurrence.title)}</h4><p>`;

  const startsAtNzTime = formatTime(eventOccurrence.startsAtNz);
  const endsAtNzTime = formatTime(eventOccurrence.endsAtNz);

  if (startsAtNzTime !== '12:00 am' && endsAtNzTime !== '12:00 am') {
    result += `<time datetime="${eventOccurrence.startsAtNz}">${startsAtNzTime}</time>`;
    result += ` - `;
    result += `<time datetime="${eventOccurrence.endsAtNz}">${endsAtNzTime}</time>`;
  } else {
    const daysDifference = getDaysDifference(eventOccurrence.startsAtNz, eventOccurrence.endsAtNz);

    if (daysDifference === 1) {
      result += `<time dateTime="${eventOccurrence.startsAtNz.slice(0, 10)}">All Day</time>`;
    } else {
      result += `<time datetime="${eventOccurrence.startsAtNz.slice(0, 10)}">${formatDate(eventOccurrence.startsAtNz)}</time>`;
      result += ` - `;
      result += `<time datetime="${eventOccurrence.endsAtNz.slice(0, 10)}">${formatDate(eventOccurrence.endsAtNz)}</time>`;
    }
  }

  result += `</p></header>`;

  result += eventOccurrence.body;

  result += getFooterHtml(eventOccurrence, 'events');

  return result;
}

export function getNoteHtml(note: Note, showOwner: boolean): string {
  let result = '';

  result += `<header>`;

  if (showOwner) {
    result += getOwnerHtml(note);
  }

  result += `<p><time datetime='${note.publishedAtNz}'>${formatDateTime(note.publishedAtNz)}</time></p>`;

  if (note.status === 'Draft') {
    result += '<p>Draft</p>';
  } else if (note.status === 'Archived') {
    result += '<p>Archived</p>';
  } else if (note.isScheduled) {
    result += '<p>Scheduled</p>';
  }

  result += `</header>`;

  result += note.body;

  result += getFooterHtml(note, 'notes');

  return result;
}

export function getUserHtml(user: User): string {
  let result = '';

  result += `<h1>${escapeHtml(user.name)}</h1>`;

  if (store.account.isAuthenticated && store.account.userId !== user.id) {
    result += `<menu role='toolbar' aria-label='User Actions'>`

    result += `<li><button type='button' aria-label='Follow' aria-pressed='${user.isFollowed ? 'true' : 'false'}'>${user.isFollowed ? 'Following' : 'Follow'}</button></li>`;

    if (store.account.isTrusted) {
      result += `<li><button type='button' aria-label='Trust' aria-pressed='${user.isTrusted ? 'true' : 'false'}'>${user.isTrusted ? 'Trusted' : 'Trust'}</button></li>`;
    } else {
      result += `<li><button type='button' aria-label='Trust' aria-pressed='${user.isTrusted ? 'true' : 'false'}' aria-describedby="trust-requirement-hint" aria-disabled="true">${user.isTrusted ? 'Trusted' : 'Trust'}</button></li>`;
    }

    result += '</menu>'

    if (!store.account.isTrusted) {
      result += '<small id="trust-requirement-hint">Trust is disabled until your account has been trusted by at least 2 other users.</small>';
    }
  }

  return result;
}

export function getWorkTime(publishedAtNz: string): string {
  let result = '<p>Published: ';

  const publishedAtNzTime = formatTime(publishedAtNz);

  if (publishedAtNzTime === '12:00 am') {
    result += `<time datetime="${publishedAtNz.slice(0, 10)}">${formatDate(publishedAtNz)}</time>`;
  } else {
    result += `<time datetime="${publishedAtNz}">${publishedAtNzTime}</time>`;
  }

  result += '</p>';

  return result;
}

export function getWorkHtml(work: Work, showOwner: boolean, heading: 'h2' | 'h3', headingIsLink: boolean): string {
  const hasOwnerHeader = work.userHeader !== null || work.spaceHeader !== null;

  let result = '';

  result += '<header>';

  result += `<${heading}>`;

  if (work.userHeader !== null) {
    if (headingIsLink) result += `<a href="/users/${work.userHeader.handle}/works/${work.handle}">`;
  } else if (work.spaceHeader !== null) {
    if (headingIsLink) result += `<a href="/spaces/${work.spaceHeader.handle}/works/${work.handle}">`;
  }

  result += escapeHtml(work.title);
  
  if (hasOwnerHeader && headingIsLink) {
    result += '</a>';
  }

  result += `</${heading}>`;

  if (showOwner) {
    result += getOwnerHtml(work);
  }

  if (work.publishedAtNz !== null) {
    result += getWorkTime(work.publishedAtNz);
  }

  result += '</header>';

  result += work.body;

  result += getFooterHtml(work, 'works');

  return result;
}

export function userButtonAction(label: string, pressed: string | null, user: User, address: string): void {
  const action = label.toLowerCase();

  if (pressed === 'true') {
    const statusActions: StatusActions = {
      204: () => {
        if (action === "follow") {
          user.isFollowed = null;
        } else if (action === "trust") {
          user.isTrusted = null;
        }
      }
    }

    const init: RequestInit = { method: "DELETE" };

    apiFetchAuthenticated({ input: `${address}/${action}`, statusActions, init });
  } else {
    const statusActions: StatusActions = {
      201: () => {
        if (action === "follow") {
          user.isFollowed = true;
        } else if (action === "trust") {
          user.isTrusted = true;
        }
      }
    }

    const init: RequestInit = { method: "POST" };

    apiFetchAuthenticated({ input: `${address}/${action}`, statusActions, init });
  }
}

export function contentButtonAction(label: string, pressed: string | null, content: Content, address: string): void {
  const action = label.toLowerCase();

  if (pressed === 'true') {
    const statusActions: StatusActions = {
      204: () => {
        if (action === "bookmark") {
          content.isBookmarked = null;
        } else if (action === "dismiss") {
          content.isDismissed = null;
        } else if (action === "star") {
          content.isStarred = null;
        }
      }
    }

    const init: RequestInit = { method: "DELETE" };

    apiFetchAuthenticated({ input: `${address}/${action}`, statusActions, init });
  } else {
    const statusActions: StatusActions = {
      201: () => {
        if (action === "bookmark") {
          content.isBookmarked = true;
        } else if (action === "dismiss") {
          content.isDismissed = true;
        } else if (action === "star") {
          content.isStarred = true;
        }
      }
    }

    const init: RequestInit = { method: "POST" };

    apiFetchAuthenticated({ input: `${address}/${action}`, statusActions, init });
  }
}
