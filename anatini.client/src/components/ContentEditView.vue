<script setup lang="ts">
  import type { ContentEdit, ErrorMessage, StatusActions } from '@/types';
  import { nextTick, ref, watch } from 'vue';
  import { useRoute } from 'vue-router';
  import { getTabIndex, markdownToHtml, paragraphToHTML, paragraphToMarkdown, parseFromString, parseSource, serializeToString, tidy, type Source } from './common/utils';
  import { apiFetchAuthenticated } from './common/apiFetch';
  import InputText from './common/InputText.vue';
  import TabButton from './common/TabButton.vue';
  import SubmitButton from './common/SubmitButton.vue';

  const headingMainRef = ref<HTMLElement | null>(null);

  const route = useRoute();

  const content = ref<ContentEdit | ErrorMessage | null>(null);
  const article = ref<Node | null>(null);
  const addText = ref<string | null>(null);
  const editText = ref<string | null>(null);
  const editIndex = ref<number | null>(null);
  const addIndex = ref<number | null>(null);
  const eTag = ref<string | null>(null);
  const tabIndex = ref<number>(0);
  const dateNZ = ref<string | null>(null);
  const name = ref<string | null>(null);
  const pageStatus = ref<string>('Loading content...');

  const tabRefs = ref<HTMLButtonElement[]>([]);

  const tabs = ref([
    { id: 'article', text: 'Article' },
    { id: 'details', text: 'Details' },
    { id: 'status', text: 'Status' }
  ]);

  watch([() => route.params.channelId, () => route.params.contentId], (source: Source) => fetchContent(parseSource(source)), { immediate: true });

  async function fetchContent(params: string[]) {
    const statusActions: StatusActions = {
      200: (response?: Response) => {
        response?.json()
          .then((value: ContentEdit) => {
            content.value = value;
            dateNZ.value = value.version.dateNZ;
            name.value = value.version.name;
            pageStatus.value = 'Ready';

            eTag.value = response.headers.get("ETag");

            if (content.value.version.article !== null) {
              const node = parseFromString(content.value.version.article);

              if (node?.nodeName === 'ARTICLE') {
                article.value = node;
              }
            }

            nextTick(() => {
              headingMainRef.value?.focus();
            });
          })
          .catch(() => { content.value = { error: true, heading: 'Unknown Error', body: 'There was a problem fetching your content, please reload the page' }});
      }
    }

    apiFetchAuthenticated(`channels/${params[0]}/posts/${params[1]}/edit`, statusActions);
  }

  function getHeading(): string {
    if (content.value === null) {
      return 'Fetching...';
    } if ('error' in content.value) {
      return content.value.heading;
    } else {
      return 'Content Edit';
    }
  }

  function setEdit(text: string, index: number): void {
    editText.value = text;
    editIndex.value = index;
  }

  function cancelSave(): void {
    editText.value = null;
    editIndex.value = null;
  }

  function addParagraph(): void {
    if (eTag.value === null || content.value === null || 'error' in content.value || addText.value === null || article.value == null) {
      return;
    }

    const node = parseFromString(markdownToHtml(`<p>${addText.value}</p>`));

    if (node !== null) {
      article.value.childNodes.forEach((value, index) => {
        if (index === addIndex.value) {
          value.after(node);
        }
      });

      const body = new FormData();
      
      body.append('article', serializeToString(article.value));

      const init = { method: "PATCH", headers: { "If-Match": eTag.value }, body: body };

      const statusActions: StatusActions = {
        204: (response?: Response) => {
          eTag.value = response?.headers.get("ETag") ?? null;

          addText.value = null;
          addIndex.value = null;
        }
      }

      apiFetchAuthenticated(`channels/${content.value.channelId}/posts/${content.value.id}`, statusActions, init);
    }
  }

  function saveParagraph(): void {
    if (eTag.value === null || content.value === null || 'error' in content.value || editText.value === null || article.value == null) {
      return;
    }

    const node = parseFromString(markdownToHtml(`<p>${editText.value}</p>`));

    if (node !== null) {
      article.value.childNodes.forEach((value, index) => {
        if (index === editIndex.value) {
          value.replaceWith(node);
        }
      });

      const body = new FormData();
      
      body.append('article', serializeToString(article.value));

      const init = { method: "PATCH", headers: { "If-Match": eTag.value }, body: body };

      const statusActions: StatusActions = {
        204: (response?: Response) => {
          eTag.value = response?.headers.get("ETag") ?? null;

          editText.value = null;
          editIndex.value = null;
        }
      }

      apiFetchAuthenticated(`channels/${content.value.channelId}/posts/${content.value.id}`, statusActions, init);
    }
  }

  function handleKeyDown(event: KeyboardEvent, index: number): void {
    const newIndex = getTabIndex(event.key, index, tabs.value.length);

    if (newIndex === undefined) {
      return;
    }

    event.preventDefault();
    tabIndex.value = newIndex;
    
    nextTick(() => {
      tabRefs.value[newIndex].focus();
    })
  }

  function setStatus(status: 'Draft' | 'Published'): void {
    if (eTag.value === null || content.value === null || 'error' in content.value) {
      return;
    }

    pageStatus.value = status === 'Draft' ? 'Unpublishing...' : 'Publishing...';

    const body = new FormData();
      
    body.append('status', status);

    const init = { method: "PATCH", headers: { "If-Match": eTag.value }, body: body };

    const statusActions: StatusActions = {
        204: (response?: Response) => {
          eTag.value = response?.headers.get("ETag") ?? null;

          if (content.value !== null && 'status' in content.value) {
            content.value.status = status;
          }

          pageStatus.value = 'Ready';
        }
    }

    apiFetchAuthenticated(`channels/${content.value.channelId}/posts/${content.value.id}`, statusActions, init);
  }

  function patchContentDetail(): void {
    if (eTag.value === null || content.value === null || 'error' in content.value) {
      return;
    }

    pageStatus.value = 'Updating...';

    const body = new FormData();

    if (name.value !== null && name.value !== content.value.version.name) {
      body.append('name', tidy(name.value));
    }

    if (dateNZ.value !== null && dateNZ.value !== content.value.version.dateNZ) {
      body.append('dateNZ', dateNZ.value);
    }

    const init = { method: "PATCH", headers: { "If-Match": eTag.value }, body: body };

    const statusActions: StatusActions = {
      204: (response?: Response) => {
        eTag.value = response?.headers.get("ETag") ?? null;

        if (content.value !== null &&  'version' in content.value) {
          if (dateNZ.value !== null) {
            content.value.version.dateNZ = dateNZ.value;            
          }

          if (name.value !== null) {
            content.value.version.name = name.value;
          }
        }

        pageStatus.value = 'Ready';
      }
    }

    apiFetchAuthenticated(`channels/${content.value.channelId}/posts/${content.value.id}`, statusActions, init);
  }

  function detailChanged(): boolean {
    if (content.value !== null && !('error' in content.value)) {
      if (name.value !== null && tidy(name.value) !== content.value.version.name) {
        return true;
      }
      else if (dateNZ.value !== null && dateNZ.value !== content.value.version.dateNZ) {
        return true;
      }
    }

    return false;
  }
</script>

<template>
  <main id="main" tabindex="-1" :aria-busy="content === null">
    <header>
      <h1 ref="headingMainRef" tabindex="-1">{{ getHeading() }}</h1>
    </header>

    <section v-if="content === null">                
      <progress max="100">Fetching content...</progress>
    </section>

    <section v-else-if="'error' in content">
      <p>
        {{ content.body }}
      </p>
    </section>

    <template v-else>
      <ul role="tablist" aria-label="Content Settings">
        <TabButton v-for="(tab, index) in tabs"
          :key="tab.id"
          :selected="tabIndex === index"
          @click="tabIndex = index"
          @keydown="handleKeyDown($event, index)"
          :text="tab.text"
          :id="tab.id"
          :add-button-ref="(el: HTMLButtonElement | null) => {if (el) tabRefs.push(el)}" />
      </ul>

      <section id="panel-article" role="tabpanel" aria-labelledby="tab-article" :hidden="tabIndex !== 0">
        <header>
          <h2>Article</h2>
        </header>

        <template v-if="article !== null">
          <section v-for="(child, index) in article.childNodes" :key="'child' + index">
            <template v-if="child.nodeName === 'P'">
              <template v-if="editText === null || editIndex !== index">
                <p v-html="paragraphToHTML(child as Element)"></p>
                <button @click="setEdit(paragraphToMarkdown(child as Element), index)" :aria-disabled="editText !== null ? true : undefined">Edit</button>
              </template>
              <template v-else>
                <InputText
                  v-model="editText"
                  name="p"
                  id="p" />
                <button @click="saveParagraph">Save</button>
                <button @click="cancelSave">Cancel</button>
              </template>
            </template>
            <template v-else>
              <p>Unknown Element</p>
            </template>
            <button @click="addIndex = index; addText = ''" v-if="addIndex === null">Add Paragraph</button>
            <template v-if="index === addIndex && addText !== null">
              <InputText
                v-model="addText"
                name = "p"
                id="p" />
              <button @click="addParagraph">Save</button>
              <button @click="addIndex = null">Cancel</button>
            </template>
          </section>
        </template>
      </section>

      <section id="panel-details" role="tabpanel" aria-labelledby="tab-details" :hidden="tabIndex !== 1">
        <header>
          <h2>Details</h2>
        </header>

        <form @submit.prevent="patchContentDetail" :action="`/api/channels/${content.channelId}/posts/${content.id}`" method="POST" novalidate>
          <fieldset>
            <legend class="visuallyhidden">Content Details</legend>

            <template v-if="name !== null">
              <InputText
                v-model="name"
                label="Name"
                name="name"
                id="name-content"
                :maxlength="64"
                :error="undefined"
                help="Article name"
                :required="true" />
            </template>

            <template v-if="dateNZ !== null">
              <label for="input-date-content">Publication Date</label>
              <input 
                type="date" 
                v-model="dateNZ"
                id="input-date-content" 
                name="date"
                aria-describedby="help-date-content"
                :aria-disabled="pageStatus !== 'Ready' ? true : undefined"
                required >

              <small id="help-date-content">
                Articles dated in the future will not be visible until that date is reached
              </small>
            </template>
          </fieldset>

          <SubmitButton
            :busy="pageStatus === 'Updating...'"
            :disabled="detailChanged() === false"
            text="Update"
            busy-text="Updating..." />
        </form>
      </section>

      <section id="panel-status" role="tabpanel" aria-labelledby="tab-status" :hidden="tabIndex !== 2">
        <header>
          <h2>Status</h2>
        </header>

        <p>This article is currently {{ content.status.toLowerCase() }}.</p>

        <p v-if="content.status === 'Published'">Republish to update with any changes.</p>

        <menu>
          <li>
            <button type="button" @click="() => setStatus('Published')" :aria-disabled="pageStatus !== 'Ready' ? true : undefined">{{ content.status === 'Published' ? 'Republish' : 'Publish' }}</button>
          </li>
          <li>
            <button type="button" @click="() => setStatus('Draft')" v-if="content.status !== 'Draft'" :aria-disabled="pageStatus !== 'Ready' ? true : undefined">Unpublish</button>
          </li>
        </menu>
      </section>
    </template>

    <p role="status" class="visuallyhidden">{{ pageStatus }}</p>
  </main>
</template>
