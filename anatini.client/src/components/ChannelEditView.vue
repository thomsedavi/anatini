<script setup lang="ts">
  import { ref, watch, useTemplateRef } from 'vue';
  import { useRoute } from 'vue-router';
  import type { ChannelEdit } from '@/types';

  const route = useRoute();

  const loading = ref<boolean>(false);
  const error = ref<string | null>(null);
  const channel = ref<ChannelEdit | null>(null);
  const isCreatingContent = ref<boolean>(false);
  const contentSlugInput = useTemplateRef<HTMLInputElement>('content-slug');
  const contentNameInput = useTemplateRef<HTMLInputElement>('content-name');

  const chosenFile = ref<File | null>(null);
  const previewUrl = ref<string | null>(null);
  const uploadStatus = ref('No file selected');

  watch([() => route.params.channelId], fetchChannel, { immediate: true });

  async function fetchChannel(array: (() => string | string[])[]) {
    error.value = channel.value = null
    loading.value = true

    fetch(`/api/channels/${array[0]}/edit`, { method: "GET" })
      .then((value: Response) => {
        if (value.ok) {
          value.json()
            .then((value: ChannelEdit) => {
              channel.value = value;
            })
            .catch(() => {
              error.value = 'Unknown Error';
            });
        } else if (value.status === 401) {
          error.value = 'Unauthorised';
        } else {
          error.value = 'Unkown Error';
        }
      })
      .catch(() => {
        error.value = 'Unknown Error';
      }).
      finally(() => {
        loading.value = false
      });
  }

  async function createContent(event: Event) {
    console.log(event);
    // event.preventDefault();

    // if (channel.value === null)
    //   return;

    // if (!validateInputs([
    //   { element: contentNameInput.value, error: 'Please enter content name.' },
    //   { element: contentSlugInput.value, error: 'Please enter content slug.' },
    // ]))
    // {
    //   return;
    // }

    // isCreatingContent.value = true;

    // const body = new FormData();

    // body.append('name', contentNameInput.value!.value.trim());
    // body.append('slug', contentSlugInput.value!.value.trim());

    // const onfulfilled = (value: ChannelEdit) => {
    //   channel.value = value;
    // };

    // const onfinally = () => {
    //   isCreatingContent.value = false;
    // };

    // const init = { method: "POST", body: body };

    // const statusActions = {
    //   409: () => {
    //     contentSlugInput.value!.setCustomValidity("Slug already in use!");
    //     reportValidity([contentSlugInput.value]);
    //   }
    // };

    // apiFetchAuthenticated(`channels/${route.params.channelId}/contents`, onfulfilled, onfinally, init, statusActions);
  }

  const onChooseFile = (event: Event) => {
    const input = event?.target as HTMLInputElement

    const file = input?.files?.[0];
  
    if (!file) return;

    if (!file.type.startsWith('image/')) {
      uploadStatus.value = 'Please select an image file';
      return;
    }

    chosenFile.value = file;
    previewUrl.value = URL.createObjectURL(file);
    uploadStatus.value = 'File selected';
  };

  // const updateDefaultCardImage = async (id: string) => {
  //   try {
  //     const formData = new FormData();

  //     formData.append('defaultCardImageId', id);

  //     const onfulfilled = () => {
  //       //loading.value = false;
  //     };

  //     const onfinally = () => {
  //       //loading.value = false;
  //     };

  //     const init: RequestInit = { method: "PATCH", body: formData };

  //     apiFetchAuthenticated(`channels/${route.params.channelId}`, onfulfilled, onfinally, init);
  //   } catch (error) {
  //     console.error(error);
  //     uploadStatus.value = "Upload failed";
  //   }
  // }

  const uploadImage = async (event: Event) => {
    console.log(event);
    // event.preventDefault();

    // if (!chosenFile.value) return;

    // uploadStatus.value = 'Uploading';

    // const formData = new FormData();

    // formData.append('file', chosenFile.value);
    // formData.append('type', 'Card');

    // try {
    //   const onfulfilled = async (value: { id: string, channelId: string }) => {
    //     updateDefaultCardImage(value.id);

    //     uploadStatus.value = 'Upload successful';

    //     chosenFile.value = null; 
    //   };

    //   const onfinally = () => {
    //     //loading.value = false;
    //   };

    //   const init: RequestInit = { method: "POST", body: formData };

    //   apiFetchAuthenticated(`channels/${route.params.channelId}/images`, onfulfilled, onfinally, init);
    // } catch (error) {
    //   console.error(error);
    //   uploadStatus.value = "Upload failed";
    // }
  }
</script>

<template>
  <main>
    <h2>ChannelEditView</h2>
    <template v-if="channel">
      <h3>{{ channel.name }}</h3>

      <form @submit.prevent="uploadImage" :action="`/api/channels/${route.params.channelId}/images`" method="POST">
        <fieldset>
          <legend>Set Default Card</legend>

          <label for="file-input">Choose File</label>
          <input
            type="file"
            accept="image/*"
            id="file-input"
            @change="onChooseFile"
            aria-describedby="file-help"
            required
            aria-controls="file-preview upload-status"
          />
          <small id="file-help">Files must be JPG or PNG, under 1MB, and have dimensions 480 wide by 360 high</small>
          <hr>

          <output id="file-preview" for="file-input">
            <figure>
              <img :src="previewUrl ?? 'https://94e01200-c64f-4ff6-87b6-ce5a316b9ea8.mdnplay.dev/shared-assets/images/examples/grapefruit-slice.jpg'" />
              <figcaption>A preview will appear here</figcaption>
            </figure>
          </output>
          <hr>

          <button type="submit" :disabled="!chosenFile">Upload</button>
          <output id="upload-status" for="file-input" aria-live="polite">{{ uploadStatus}}</output>

          <footer>
            <small>This will be the default preview image for content</small>
          </footer>
        </fieldset>
      </form>

      <form @submit.prevent="createContent" action="/api/channels/lotographia/contents" method="POST">
        <fieldset>
          <legend>Create Content</legend>

          <label for="contentName">Content Name</label>
          <input id="contentName" type="text" name="name" maxlength="64" ref="content-name" @input="event => contentNameInput?.setCustomValidity('')">

          <hr>

          <label for="contentSlug">Content Slug</label>
          <input id="contentSlug" type="text" name="slug" maxlength="64" ref="content-slug" @input="event => contentSlugInput?.setCustomValidity('')">

          <hr>

          <button type="submit" :disabled="isCreatingContent">Submit</button>
        </fieldset>
      </form>
      <template v-if="channel.topDraftContents?.length">
        <div v-for="(content, index) in channel.topDraftContents" :key="'topDraftContent' + index">
          <RouterLink :to="{ name: 'ContentEdit', params: { channelId: route.params.channelId, contentId: content.defaultSlug }}">{{ content.name }}: Edit</RouterLink>
          <RouterLink :to="{ name: 'ContentPreview', params: { channelId: route.params.channelId, contentId: content.defaultSlug }}">{{ content.name }}: Preview</RouterLink>
        </div>
      </template>
    </template>
  </main>
</template>
