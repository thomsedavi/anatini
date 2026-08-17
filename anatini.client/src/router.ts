import { createWebHistory, createRouter, type RouteRecordRaw } from 'vue-router'

import AboutView from './components/AboutView.vue';
import SignInView from './components/SignInView.vue';
import AccountView from './components/AccountView.vue';
import AccountPublicView from './components/account/AccountPublicView.vue';
import AccountPrivateView from './components/account/AccountPrivateView.vue';
import AccountSpacesView from './components/account/AccountSpacesView.vue';
import SignUpView from './components/SignUpView.vue';
import SpaceView from './components/SpaceView.vue';
import TagsView from './components/TagsView.vue';

import HomeView from './components/HomeView.vue';
import HomePostsView from './components/home/posts/HomePostsView.vue';
import HomeEventsView from './components/home/events/HomeEventsView.vue';
import HomeWorksView from './components/home/works/HomeWorksView.vue';

import UsersView from './components/users/UsersView.vue';
import UserView from './components/users/user/UserView.vue';
import UserPostsView from './components/users/user/posts/UserPostsView.vue';
import UserEventsView from './components/users/user/events/UserEventsView.vue';
import UserWorksView from './components/users/user/works/UserWorksView.vue';
import UserWebsiteView from './components/users/user/works/website/UserWebsiteView.vue';
import UserWebsiteCreateView from './components/users/user/works/website/UserWebsiteCreateView.vue';
import UserProductView from './components/users/user/works/product/UserProductView.vue';
import UserProductCreateView from './components/users/user/works/product/UserProductCreateView.vue';
import UserProjectView from './components/users/user/works/project/UserProjectView.vue';
import UserProjectCreateView from './components/users/user/works/project/UserProjectCreateView.vue';
import UserNoteView from './components/users/user/posts/note/UserNoteView.vue';
import UserNoteCreateView from './components/users/user/posts/note/UserNoteCreateView.vue';
import UserNoteEditView from './components/users/user/posts/note/UserNoteEditView.vue';
import UserEventView from './components/users/user/events/event/UserEventView.vue';
import UserEventCreateView from './components/users/user/events/event/UserEventCreateView.vue';
import UserEventOccurrenceView from './components/users/user/events/event/UserEventOccurrenceView.vue';

import SpaceCreateView from './components/SpaceCreateView.vue';
import SpaceEditView from './components/spaceEdit/SpaceEditView.vue';
import SpaceEditNoteCreateView from './components/spaceEdit/SpaceEditNoteCreateView.vue';
import SpaceEditDisplayView from './components/spaceEdit/SpaceEditDisplayView.vue';
import SpaceEditNotesView from './components/spaceEdit/SpaceEditNotesView.vue';
import SpaceEditNoteEditView from './components/spaceEdit/SpaceEditNoteEditView.vue';
import SpaceEditPostsView from './components/spaceEdit/SpaceEditPostsView.vue';
import SpaceNotesView from './components/spaces/space/notes/SpaceNotesView.vue';
import SpaceNoteView from './components/spaces/space/notes/note/SpaceNoteView.vue';
import SpaceNoteCreateView from './components/spaces/space/notes/note/SpaceNoteCreateView.vue';
import SpaceNoteEditView from './components/spaces/space/notes/note/SpaceNoteEditView.vue';
import SpaceEventsView from './components/spaces/space/events/SpaceEventsView.vue';
import SpaceEventCreateView from './components/spaces/space/events/event/SpaceEventCreateView.vue';

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
        path: 'home/events',
        component: HomeEventsView,
        name: 'HomeEvents'
      },
      {
        path: 'home/works',
        component: HomeWorksView,
        name: 'HomeWorks'
      },
    ],
  },
  {
    path: '/about',
    component: AboutView,
  },
  {
    path: '/sign-up',
    component: SignUpView,
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
    ],
  },
  {
    path: '/users/:userId',
    component: UserView,
    name: 'User',
    redirect: { name: 'UserPosts' },
    children: [
      {
        path: 'notes/create',
        component: UserNoteCreateView,
        name: 'UserNoteCreate'
      },
      {
        path: 'notes/:noteId/edit',
        component: UserNoteEditView,
        name: 'UserNoteEdit'
      },
      {
        path: 'notes/:noteId',
        component: UserNoteView,
        name: 'UserNote'
      },
      {
        path: 'posts',
        component: UserPostsView,
        name: 'UserPosts'
      },
      {
        path: 'events/create',
        component: UserEventCreateView,
        name: 'UserEventCreate'
      },
      {
        path: 'events/:eventId/occurrence/:occurrenceId',
        component: UserEventOccurrenceView,
        name: 'EventOccurrence',
      },
      {
        path: 'events/:eventId',
        component: UserEventView,
        name: 'Event',
      },
      {
        path: 'events',
        component: UserEventsView,
        name: 'UserEvents'
      },
      {
        path: 'works',
        component: UserWorksView,
        name: 'UserWorks'
      },
      {
        path: 'websites/create',
        component: UserWebsiteCreateView,
        name: 'UserWebsiteCreate'
      },
      {
        path: 'website/:websiteId',
        component: UserWebsiteView,
        name: 'UserWebsite'
      },
      {
        path: 'products/create',
        component: UserProductCreateView,
        name: 'UserProductCreate'
      },
      {
        path: 'product/:productId',
        component: UserProductView,
        name: 'UserProduct'
      },
      {
        path: 'projects/create',
        component: UserProjectCreateView,
        name: 'UserProjectCreate'
      },
      {
        path: 'project/:projectId',
        component: UserProjectView,
        name: 'UserProject'
      },
    ],
  },
  {
    path: '/spaces/create',
    component: SpaceCreateView,
    name: 'SpaceCreate',
  },
  {
    path: '/spaces/:spaceId',
    component: SpaceView,
    name: 'Space',
    redirect: { name: 'SpaceNotes' },
    children: [
      {
        path: 'notes/create',
        component: SpaceNoteCreateView,
        name: 'SpaceNoteCreate'
      },
      {
        path: 'notes/:noteId/edit',
        component: SpaceNoteEditView,
        name: 'SpaceNoteEdit'
      },
      {
        path: 'notes/:noteId',
        component: SpaceNoteView,
        name: 'SpaceNote'
      },
      {
        path: 'notes',
        component: SpaceNotesView,
        name: 'SpaceNotes'
      },
      {
        path: 'events/create',
        component: SpaceEventCreateView,
        name: 'SpaceEventCreate'
      },
      {
        path: 'events',
        component: SpaceEventsView,
        name: 'SpaceEvents'
      },
    ],
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
