import { createWebHistory, createRouter } from 'vue-router'

import AboutView from './components/AboutView.vue';
import HomeView from './components/HomeView.vue';
import LoginView from './components/LoginView.vue';
import PostCreateView from './components/PostCreateView.vue';
import NoteCreateView from './components/NoteCreateView.vue';
import NoteView from './components/NoteView.vue';
import PostEditView from './components/PostEditView.vue';
import PostPreviewView from './components/PostPreviewView.vue';
import PostView from './components/PostView.vue';
import AccountView from './components/account/AccountView.vue';
import SignupFlowView from './components/SignupFlowView.vue';
import UserView from './components/UserView.vue';
import ChannelCreateView from './components/ChannelCreateView.vue';
import ChannelEditView from './components/channelEdit/ChannelEditView.vue';
import ChannelEditNotesView from './components/channelEdit/ChannelEditNotesView.vue';
import ChannelEditPostsView from './components/channelEdit/ChannelEditPostsView.vue';
import ChannelEditDisplayView from './components/channelEdit/ChannelEditDisplayView.vue';
import ChannelView from './components/ChannelView.vue';
import UsersView from './components/UsersView.vue';
import TagsView from './components/TagsView.vue';

const routes = [
  {
    path: '/',
    component: HomeView,
  },
  {
    path: '/about',
    component: AboutView,
  },
  {
    path: '/signup',
    component: SignupFlowView,
  },
  {
    path: '/login',
    component: LoginView,
  },
  {
    path: '/account',
    component: AccountView,
  },
  {
    path: '/channels/:channelId/posts/create',
    component: PostCreateView,
    name: 'PostCreate',
  },
  {
    path: '/channels/:channelId/notes/create',
    component: NoteCreateView,
    name: 'NoteCreate',
  },
  {
    path: '/channels/:channelId/posts/:postId/edit',
    component: PostEditView,
    name: 'PostEdit',
  },
  {
    path: '/channels/:channelId/posts/:postId/preview',
    component: PostPreviewView,
    name: 'PostPreview',
  },
  {
    path: '/channels/:channelId/posts/:postId',
    component: PostView,
    name: 'Post',
  },
  {
    path: '/channels/:channelId/notes/:noteId',
    component: NoteView,
    name: 'Note',
  },
  {
    path: '/users/:userId',
    component: UserView,
  },
  {
    path: '/channels/create',
    component: ChannelCreateView,
    name: 'ChannelCreate',
  },
  {
    path: '/channels/:channelId/edit',
    component: ChannelEditView,
    name: 'ChannelEdit',
    redirect: { name: 'ChannelEditPosts' },
    children: [
      {
        path: 'posts',
        component: ChannelEditPostsView,
        name: 'ChannelEditPosts'
      },
      {
        path: 'notes',
        component: ChannelEditNotesView,
        name: 'ChannelEditNotes'
      },
      {
        path: 'display',
        component: ChannelEditDisplayView,
        name: 'ChannelEditDisplay'
      },
    ],
  },
  {
    path: '/channels/:channelId',
    component: ChannelView,
    name: 'Channel',
  },
   {
    path: '/tags/:tagId',
    component: TagsView,
    name: 'Tags',
  },
  {
    path: '/users',
    component: UsersView,
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
  linkActiveClass: '',
  linkExactActiveClass: '',
})

export default router
