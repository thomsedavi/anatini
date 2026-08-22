<script setup lang="ts">
  import { apiFetch, apiFetchAuthenticated } from '@/common/apiFetch';
  import { store } from '@/common/store';
  import type { APIResponse, StatusActions, Work } from '@/common/types';
  import { handleClick, parseSource, type Source } from '@/common/utils';
  import { ref, watch } from 'vue';
  import { useRoute, useRouter } from 'vue-router';

  const route = useRoute();
  const router = useRouter();

  const props = defineProps<{
    dataUserId: string,
    dataUserHandle: string,
    dataUserName: string,
  }>();

  const website = ref<APIResponse<Work>>({ fetching: true });

  watch([() => route.params.userId, () => route.params.websiteId], (source: Source) => fetchWebsite(parseSource(source)), { immediate: true });

  async function fetchWebsite(params: string[]) {
    const input = `users/${params[0]}/websites/${params[1]}`;

    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: Work) => {
            website.value = { data: value };
          })
          .catch(() => {
            website.value = { error: { heading: 'Unknown Error', body: 'There was a problem fetching your website, please reload the page' }};
          });
      },
      404: () => {
        website.value = { error: { heading: '404 Not Found', body: 'Website not found' }};
      }
    }

    apiFetch({ input, statusActions });
  }

  function buttonAction(label: string, pressed: string | null): void {
    const action = label.toLowerCase();

    if (pressed === 'true') {
      const statusActions: StatusActions = {
        204: () => {
          if (action === "bookmark") {
            website.value.data!.hasBookmarked = null;
          } else if (action === "dismiss") {
            website.value.data!.hasDismissed = null;
          } else if (action === "star") {
            website.value.data!.hasStarred = null;
          }
        }
      }

      const init: RequestInit = { method: "DELETE" };

      apiFetchAuthenticated({ input: `users/${props.dataUserId}/websites/${website.value.data!.id}/${action}`, statusActions, init });
    } else {
      const statusActions: StatusActions = {
        201: () => {
          if (action === "bookmark") {
            website.value.data!.hasBookmarked = true;
          } else if (action === "dismiss") {
            website.value.data!.hasDismissed = true;
          } else if (action === "star") {
            website.value.data!.hasStarred = true;
          }
        }
      }

      const init: RequestInit = { method: "POST" };

      apiFetchAuthenticated({ input: `users/${props.dataUserId}/websites/${website.value.data!.id}/${action}`, statusActions, init });
    }
  }

  function articleHtml(): string {
    if (website.value.data !== undefined) {
      const websiteData = website.value.data;

      return `
        <header>
          <h2>${ websiteData.name ?? 'Loading' }</h2>
        </header>
        ${websiteData.article !== null ? websiteData.article.substring(9, websiteData.article.length - 10) : ''}
        ${store.isAuthenticated === true ? `<footer>
          <menu>
            <li>
              <button type='button' aria-label='Dismiss' aria-pressed='${websiteData.hasDismissed ? 'true' : 'false'}'>${websiteData.hasDismissed ? '<svg width="1em" height="1em" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24" /><line x1="1" y1="1" x2="23" y2="23" /></svg>' : '<svg width="1em" height="1em" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z" /><circle cx="12" cy="12" r="3" /></svg>'}</button>
            </li>
            <li>
              <button type='button' aria-label='Star' aria-pressed='${websiteData.hasStarred ? 'true' : 'false'}'>${websiteData.hasStarred ? '<svg width="1em" height="1em" viewBox="0 0 24 24" fill="currentColor" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"/></svg>' : '<svg width="1em" height="1em" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"/></svg>'}</button>
            </li>
            <li>
              <button type='button' aria-label='Bookmark' aria-pressed='${websiteData.hasBookmarked ? 'true' : 'false'}'>${websiteData.hasBookmarked ? '<svg width="1em" height="1em" viewBox="0 0 24 24" fill="currentColor" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><path d="M19 21l-7-5-7 5V5a2 2 0 0 1 2-2h10a2 2 0 0 1 2 2z" /></svg>' : '<svg width="1em" height="1em" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false"><path d="M19 21l-7-5-7 5V5a2 2 0 0 1 2-2h10a2 2 0 0 1 2 2z" /></svg>'}</button>
            </li>
          </menu>
        </footer>` : ''}
      `;
    }

    return '';
  }
</script>

<template>
  <section id="panel-works" role="tabpanel" aria-labelledby="tab-works" tabindex="0">
    <nav aria-label="Breadcrumb">
      <ol role="list">
        <li>
          <RouterLink :to="{ name: 'UserWorks', params: { userId: dataUserHandle } }"><span aria-hidden="true">&larr;</span> Back to all works by {{ dataUserName }}</RouterLink>
        </li>
      </ol>
    </nav>

    <article v-html="articleHtml()" @click.prevent="(mouseEvent) => handleClick(mouseEvent, router, (label, pressed) => buttonAction(label, pressed))">
    </article>
  </section>
</template>
