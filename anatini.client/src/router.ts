import { createWebHistory, createRouter, type RouteRecordRaw } from 'vue-router'

import AboutView from './components/AboutView.vue';
import HomeView from './components/home/HomeView.vue';
import HomePostsView from './components/home/HomePostsView.vue';
import HomeNotesView from './components/home/HomeNotesView.vue';
import HomeCalendarView from './components/home/HomeCalendarView.vue';
import SignInView from './components/SignInView.vue';
import PostCreateView from './components/PostCreateView.vue';
import NoteView from './components/notes/NoteView.vue';
import UserEventView from './components/events/UserEventView.vue';
import PostEditView from './components/postEdit/PostEditView.vue';
import PostEditArticleView from './components/postEdit/PostEditArticleView.vue';
import PostEditDetailsView from './components/postEdit/PostEditDetailsView.vue';
import PostEditStatusView from './components/postEdit/PostEditStatusView.vue';
import PostPreviewView from './components/PostPreviewView.vue';
import PostView from './components/PostView.vue';
import AccountView from './components/account/AccountView.vue';
import AccountPublicView from './components/account/AccountPublicView.vue';
import AccountNotesView from './components/account/AccountNotesView.vue';
import AccountCalendarView from './components/account/AccountCalendarView.vue';
import AccountNoteCreateView from './components/account/AccountNoteCreateView.vue';
import AccountEventCreateView from './components/account/AccountEventCreateView.vue';
import AccountNoteEditView from './components/account/AccountNoteEditView.vue';
import AccountPrivateView from './components/account/AccountPrivateView.vue';
import AccountSpacesView from './components/account/AccountSpacesView.vue';
import SignUpFlowView from './components/SignUpFlowView.vue';
import UserView from './components/UserView.vue';
import SpaceCreateView from './components/SpaceCreateView.vue';
import SpaceEditView from './components/spaceEdit/SpaceEditView.vue';
import SpaceEditNotesView from './components/spaceEdit/SpaceEditNotesView.vue';
import SpaceEditNoteCreateView from './components/spaceEdit/SpaceEditNoteCreateView.vue';
import SpaceEditNoteEditView from './components/spaceEdit/SpaceEditNoteEditView.vue';
import SpaceEditPostsView from './components/spaceEdit/SpaceEditPostsView.vue';
import SpaceEditDisplayView from './components/spaceEdit/SpaceEditDisplayView.vue';
import SpaceView from './components/SpaceView.vue';
import UsersView from './components/UsersView.vue';
import TagsView from './components/TagsView.vue';

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    component: HomeView,
    name: 'Home',
    redirect: { name: 'HomePosts' },
    children: [
      {
        path: 'home/posts',
        component: HomePostsView,
        name: 'HomePosts'
      },
      {
        path: 'home/notes',
        component: HomeNotesView,
        name: 'HomeNotes'
      },
      {
        path: 'home/calendar',
        component: HomeCalendarView,
        name: 'HomeCalendar'
      },
    ],
  },
  {
    path: '/about',
    component: AboutView,
  },
  {
    path: '/sign-up',
    component: SignUpFlowView,
  },
  {
    path: '/sign-in',
    component: SignInView,
  },
  {
    path: '/account',
    component: AccountView,
    name: 'Account',
    redirect: { name: 'AccountPublic' },
    children: [
      {
        path: 'public',
        component: AccountPublicView,
        name: 'AccountPublic'
      },
      {
        path: 'private',
        component: AccountPrivateView,
        name: 'AccountPrivate'
      },
      {
        path: 'spaces',
        component: AccountSpacesView,
        name: 'AccountSpaces'
      },
      {
        path: 'notes/create',
        component: AccountNoteCreateView,
        name: 'AccountNoteCreate'
      },
      {
        path: 'events/create',
        component: AccountEventCreateView,
        name: 'AccountEventCreate'
      },
      {
        path: 'notes/:noteId/edit',
        component: AccountNoteEditView,
        name: 'AccountNoteEdit'
      },
      {
        path: 'notes',
        component: AccountNotesView,
        name: 'AccountNotes'
      },
      {
        path: 'calendar',
        component: AccountCalendarView,
        name: 'AccountCalendar'
      },

    ],
  },
  {
    path: '/spaces/:spaceId/posts/create',
    component: PostCreateView,
    name: 'PostCreate',
  },
  {
    path: '/spaces/:spaceId/posts/:postId/edit',
    component: PostEditView,
    name: 'PostEdit',
    redirect: { name: 'PostEditArticle' },
    children: [
      {
        path: 'article',
        component: PostEditArticleView,
        name: 'PostEditArticle'
      },
      {
        path: 'details',
        component: PostEditDetailsView,
        name: 'PostEditDetails'
      },
      {
        path: 'status',
        component: PostEditStatusView,
        name: 'PostEditStatus'
      },
    ],
  },
  {
    path: '/spaces/:spaceId/posts/:postId/preview',
    component: PostPreviewView,
    name: 'PostPreview',
  },
  {
    path: '/spaces/:spaceId/posts/:postId',
    component: PostView,
    name: 'Post',
  },
  {
    path: '/spaces/:spaceId/notes/:noteId',
    component: NoteView,
    name: 'Note',
  },
  {
    path: '/users/:userId/events/:eventId',
    component: UserEventView,
    name: 'Event',
  },
  {
    path: '/users/:userId',
    component: UserView,
  },
  {
    path: '/spaces/create',
    component: SpaceCreateView,
    name: 'SpaceCreate',
  },
  {
    path: '/spaces/:spaceId/edit',
    component: SpaceEditView,
    name: 'SpaceEdit',
    redirect: { name: 'SpaceEditPosts' },
    children: [
      {
        path: 'posts',
        component: SpaceEditPostsView,
        name: 'SpaceEditPosts'
      },
      {
        path: 'notes/create',
        component: SpaceEditNoteCreateView,
        name: 'SpaceEditNoteCreate'
      },
      {
        path: 'notes/:noteId/edit',
        component: SpaceEditNoteEditView,
        name: 'SpaceEditNoteEdit'
      },
      {
        path: 'notes',
        component: SpaceEditNotesView,
        name: 'SpaceEditNotes'
      },
      {
        path: 'display',
        component: SpaceEditDisplayView,
        name: 'SpaceEditDisplay'
      },
    ],
  },
  {
    path: '/spaces/:spaceId',
    component: SpaceView,
    name: 'Space',
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
