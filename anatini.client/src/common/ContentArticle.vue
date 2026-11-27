<script setup lang="ts">
  import { apiFetchAuthenticated } from "@/common/apiFetch";
  import { formatDateTime, formatShortestDateTime } from "@/common/dateUtils";
  import { store } from "@/common/store";
  import type { Content, ContentType, StatusActions } from '@/common/types';
  import { ContentBody, getContentAddress, getContentStatus, getWorkDateTime, isNote, isWork, pascalToKebab } from '@/common/utils';

  const props = defineProps<{
    content: Content,
    contentType: ContentType,
    isMain: boolean,
    index?: number,
  }>();

  const emit = defineEmits<{
    'update-content': [content: Content],
  }>();

  const contentTypeKebab = pascalToKebab(props.contentType);
  const contentIdPrefix = props.index === undefined ? contentTypeKebab : `${contentTypeKebab}-${props.index}`;

  function ownerButtonAction(action: string, isPressed: boolean | null): void {
    const ownerHeader = props.content.ownerHeader;

    if (ownerHeader !== null) {
      const ownerAddress = `${pascalToKebab(ownerHeader.type)}s/${ownerHeader.id}`;

      if (isPressed) {
        const statusActions: StatusActions = {
          204: () => {
            if (action === "follow") {
              emit('update-content', { ...props.content, ownerHeader: { ...ownerHeader, isFollowed: false }});
            }
          }
        }

        const init: RequestInit = { method: "DELETE" };

        apiFetchAuthenticated({ input: `${ownerAddress}/${action}`, statusActions, init });
      } else {
        const statusActions: StatusActions = {
          201: () => {
            if (action === "follow") {
              emit('update-content', { ...props.content, ownerHeader: { ...ownerHeader, isFollowed: true }});
            }
          }
        }

        const init: RequestInit = { method: "POST" };

        apiFetchAuthenticated({ input: `${ownerAddress}/${action}`, statusActions, init });
      }
    }
  }

  function contentButtonAction(action: string, isPressed: boolean | null): void {
    const contentAddress = getContentAddress(props.content, props.contentType, 'id');

    if (isPressed) {
      const statusActions: StatusActions = {
        204: () => {
          if (action === "bookmark") {
            emit('update-content', { ...props.content, isBookmarked: false });
          } else if (action === "dismiss") {
            emit('update-content', { ...props.content, isDismissed: false });
          } else if (action === "star") {
            emit('update-content', { ...props.content, isStarred: false });
          }
        }
      }

      const init: RequestInit = { method: "DELETE" };

      apiFetchAuthenticated({ input: `${contentAddress}/${action}`, statusActions, init });
    } else {
      const statusActions: StatusActions = {
        201: () => {
          if (action === "bookmark") {
            emit('update-content', { ...props.content, isBookmarked: true });
          } else if (action === "dismiss") {
            emit('update-content', { ...props.content, isDismissed: true });
          } else if (action === "star") {
            emit('update-content', { ...props.content, isStarred: true });
          }
        }
      }

      const init: RequestInit = { method: "POST" };

      apiFetchAuthenticated({ input: `${contentAddress}/${action}`, statusActions, init });
    }
  }
</script>

<template>
  <article :data-status="getContentStatus(content)">
    <header>
      <template v-if="isWork(content, contentType)">
        <h2 v-if="isMain">{{ content.title }}</h2>
        <h3 v-else-if="content.ownerHeader !== null"><RouterLink :to="{ name: `${content.ownerHeader.type}${contentType}`, params: { [`${content.ownerHeader.type.toLowerCase()}Id`]: content.ownerHeader.handle, [`${contentTypeKebab}Id`]: content.handle } }">{{ content.title }}</RouterLink></h3>
        <h3 v-else>{{ content.title }}</h3>
      </template>
      <address v-if="!isMain && content.ownerHeader !== null" role="presentation">
        <RouterLink :to="{ name: content.ownerHeader.type, params: { [`${content.ownerHeader.type.toLowerCase()}Id`]: content.ownerHeader.handle } }">{{ content.ownerHeader.name }}</RouterLink>
      </address>
      <p v-if="isNote(content, contentType)">
        <time :datetime="content.publishedAtNz">{{ formatDateTime(content.publishedAtNz) }}</time>
      </p>
      <p v-if="isWork(content, contentType) && content.publishedAtNz !== null">
        Published: <time :datetime="getWorkDateTime(content.publishedAtNz)">{{ formatShortestDateTime(content.publishedAtNz) }}</time>
      </p>
      <p v-if="content.status === 'Draft'">Draft</p>
      <p v-else-if="content.status === 'Archived'">Archived</p>
      <p v-else-if="content.isScheduled">Scheduled</p>
      <button type="button" :popovertarget="`${contentIdPrefix}-additional-actions`">…</button>
      <menu :id="`${contentIdPrefix}-additional-actions`" popover="auto" aria-label="Additional actions">
        <li v-if="!isMain && content.ownerHeader !== null">
          <button :data-entity-type="content.ownerHeader.type" :data-entity-id="content.ownerHeader.id" aria-label="Follow" :aria-pressed="content.ownerHeader.isFollowed ?? false" @click="() => ownerButtonAction('follow', content.ownerHeader!.isFollowed)">{{ content.ownerHeader!.isFollowed ? 'Unfollow' : 'Follow' }}</button>
        </li>
        <li v-if="content.isOwned && content.ownerHeader !== null">
          <RouterLink :to="{ name: `${content.ownerHeader.type}${contentType}Edit`, params: { [`${content.ownerHeader.type.toLowerCase()}Id`]: content.ownerHeader.handle, [`${contentTypeKebab}Id`]: content.handle } }">Edit</RouterLink>
        </li>
      </menu>
    </header>
    <ContentBody :html="content.body" />
    <footer v-if="store.account.isAuthenticated">
      <menu role="toolbar" aria-label="Actions">
        <li>
          <button type="button" aria-label="Star" :aria-pressed="content.isStarred ?? false" @click="() => contentButtonAction('star', content.isStarred)">
            <svg v-if="content.isStarred" width="1em" height="1em" viewBox="0 0 24 24" fill="currentColor" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"/></svg>
            <svg v-else width="1em" height="1em" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"/></svg>
          </button>
        </li>
        <li>
          <button type="button" aria-label="Bookmark" :aria-pressed="content.isBookmarked ?? false" @click="() => contentButtonAction('bookmark', content.isBookmarked)">
            <svg v-if="content.isBookmarked" width="1em" height="1em" viewBox="0 0 24 24" fill="currentColor" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><path d="M19 21l-7-5-7 5V5a2 2 0 0 1 2-2h10a2 2 0 0 1 2 2z" /></svg>
            <svg v-else width="1em" height="1em" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><path d="M19 21l-7-5-7 5V5a2 2 0 0 1 2-2h10a2 2 0 0 1 2 2z" /></svg>
          </button>
        </li>
        <li>
          <button type="button" aria-label="Dismiss" :aria-pressed="content.isDismissed ?? false" @click="() => contentButtonAction('dismiss', content.isDismissed)">
            <svg v-if="content.isDismissed" width="1em" height="1em" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24" /><line x1="1" y1="1" x2="23" y2="23" /></svg>
            <svg v-else width="1em" height="1em" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z" /><circle cx="12" cy="12" r="3" /></svg>
          </button>
        </li>
      </menu>
    </footer>
  </article>
</template>
