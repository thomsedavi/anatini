import { createWebHistory, createRouter, type RouteRecordRaw } from 'vue-router'

import AboutView from './components/AboutView.vue';
import SignInView from './components/SignInView.vue';
import AccountView from './components/AccountView.vue';
import AccountPublicView from './components/account/AccountPublicView.vue';
import AccountPrivateView from './components/account/AccountPrivateView.vue';
import AccountSpacesView from './components/account/AccountSpacesView.vue';
import SignUpView from './components/SignUpView.vue';
import TagsView from './components/TagsView.vue';

import HomeView from './components/home/HomeView.vue';
import HomeNotesView from './components/home/notes/HomeNotesView.vue';
import HomeEventsView from './components/home/events/HomeEventsView.vue';
import HomeWorksView from './components/home/works/HomeWorksView.vue';

import UsersView from './components/users/UsersView.vue';
import UserView from './components/users/user/UserView.vue';
import UserNotesView from './components/users/user/notes/UserNotesView.vue';
import UserEventsView from './components/users/user/events/UserEventsView.vue';
import UserWorksView from './components/users/user/works/UserWorksView.vue';
import UserWorkView from './components/users/user/works/work/UserWorkView.vue';
import UserWorkCreateView from './components/users/user/works/work/UserWorkCreateView.vue';
import UserWorkEditView from './components/users/user/works/work/UserWorkEditView.vue';
import UserNoteView from './components/users/user/notes/note/UserNoteView.vue';
import UserNoteCreateView from './components/users/user/notes/note/UserNoteCreateView.vue';
import UserNoteEditView from './components/users/user/notes/note/UserNoteEditView.vue';
import UserEventView from './components/users/user/events/event/UserEventView.vue';
import UserEventCreateView from './components/users/user/events/event/UserEventCreateView.vue';
// import UserEventOccurrenceView from './components/users/user/events/event/UserEventOccurrenceView.vue';
import UserSpacesView from './components/users/user/spaces/UserSpacesView.vue';
import UserSpaceCreateView from './components/users/user/spaces/space/UserSpaceCreateView.vue';

import SpaceView from './components/spaces/space/SpaceView.vue';
import SpaceCreateView from './components/spaces/space/SpaceCreateView.vue';
import SpaceEditView from './components/spaceEdit/SpaceEditView.vue';
import SpaceEditNoteCreateView from './components/spaceEdit/SpaceEditNoteCreateView.vue';
import SpaceEditDisplayView from './components/spaceEdit/SpaceEditDisplayView.vue';
import SpaceEditNoteEditView from './components/spaceEdit/SpaceEditNoteEditView.vue';
import SpaceEditNotesView from './components/spaceEdit/SpaceEditNotesView.vue';
import SpaceWorksView from './components/spaces/space/works/SpaceWorksView.vue';
import SpaceWorkView from './components/spaces/space/works/work/SpaceWorkView.vue';
import SpaceWorkCreateView from './components/spaces/space/works/work/SpaceWorkCreateView.vue';
import SpaceWorkEditView from './components/spaces/space/works/work/SpaceWorkEditView.vue';
import SpaceNotesView from './components/spaces/space/notes/SpaceNotesView.vue';
import SpaceNoteView from './components/spaces/space/notes/note/SpaceNoteView.vue';
import SpaceNoteCreateView from './components/spaces/space/notes/note/SpaceNoteCreateView.vue';
import SpaceNoteEditView from './components/spaces/space/notes/note/SpaceNoteEditView.vue';
import SpaceEventsView from './components/spaces/space/events/SpaceEventsView.vue';
import SpaceEventView from './components/spaces/space/events/event/SpaceEventView.vue';
import SpaceEventCreateView from './components/spaces/space/events/event/SpaceEventCreateView.vue';

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    component: HomeView,
    name: 'Home',
    redirect: { name: 'HomeNotes' },
    children: [
      {
        path: 'home/notes',
        component: HomeNotesView,
        name: 'HomeNotes'
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
    redirect: { name: 'UserNotes' },
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
        path: 'notes',
        component: UserNotesView,
        name: 'UserNotes'
      },
      {
        path: 'events/create',
        component: UserEventCreateView,
        name: 'UserEventCreate'
      },
      // {
      //   path: 'events/:eventId/occurrence/:occurrenceId',
      //   component: UserEventOccurrenceView,
      //   name: 'EventOccurrence',
      // },
      {
        path: 'events/:eventId',
        component: UserEventView,
        name: 'UserEvent',
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
        path: 'works/create',
        component: UserWorkCreateView,
        name: 'UserWorkCreate'
      },
      {
        path: 'works/:workId/edit',
        component: UserWorkEditView,
        name: 'UserWorkEdit'
      },
      {
        path: 'works/:workId',
        component: UserWorkView,
        name: 'UserWork'
      },
      {
        path: 'spaces',
        component: UserSpacesView,
        name: 'UserSpaces'
      },
      {
        path: 'spaces/create',
        component: UserSpaceCreateView,
        name: 'UserSpaceCreate'
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
        path: 'events/:eventId',
        component: SpaceEventView,
        name: 'SpaceEvent'
      },
      {
        path: 'events',
        component: SpaceEventsView,
        name: 'SpaceEvents'
      },
      {
        path: 'works',
        component: SpaceWorksView,
        name: 'SpaceWorks'
      },
      {
        path: 'works/create',
        component: SpaceWorkCreateView,
        name: 'SpaceWorkCreate'
      },
      {
        path: 'works/:workId/edit',
        component: SpaceWorkEditView,
        name: 'SpaceWorkEdit'
      },
      {
        path: 'works/:workId',
        component: SpaceWorkView,
        name: 'SpaceWork'
      },
    ],
  },
  {
    path: '/spaces/:spaceId/edit',
    component: SpaceEditView,
    name: 'SpaceEdit',
    redirect: { name: 'SpaceEditNotes' },
    children: [
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
