import { createWebHistory, createRouter, type RouteRecordRaw } from 'vue-router'

import AboutView from './components/AboutView.vue';
import SignInView from './components/SignInView.vue';
import AccountView from './components/AccountView.vue';
import AccountPublicView from './components/account/AccountPublicView.vue';
import AccountPrivateView from './components/account/AccountPrivateView.vue';
import AccountSpacesView from './components/account/AccountSpacesView.vue';
import SignUpView from './components/SignUpView.vue';
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
import UserWorkView from './components/users/user/works/work/UserWorkView.vue';
import UserWorkCreateView from './components/users/user/works/work/UserWorkCreateView.vue';
import UserWorkEditView from './components/users/user/works/work/UserWorkEditView.vue';
import UserPostView from './components/users/user/posts/post/UserPostView.vue';
import UserPostCreateView from './components/users/user/posts/post/UserPostCreateView.vue';
import UserPostEditView from './components/users/user/posts/post/UserPostEditView.vue';
import UserEventView from './components/users/user/events/event/UserEventView.vue';
import UserEventCreateView from './components/users/user/events/event/UserEventCreateView.vue';
import UserEventOccurrenceView from './components/users/user/events/event/UserEventOccurrenceView.vue';

import SpaceView from './components/spaces/space/SpaceView.vue';
import SpaceCreateView from './components/spaces/space/SpaceCreateView.vue';
import SpaceEditView from './components/spaceEdit/SpaceEditView.vue';
import SpaceEditPostCreateView from './components/spaceEdit/SpaceEditPostCreateView.vue';
import SpaceEditDisplayView from './components/spaceEdit/SpaceEditDisplayView.vue';
import SpaceEditPostEditView from './components/spaceEdit/SpaceEditPostEditView.vue';
import SpaceEditPostsView from './components/spaceEdit/SpaceEditPostsView.vue';
import SpaceWorksView from './components/spaces/space/works/SpaceWorksView.vue';
import SpaceWorkView from './components/spaces/space/works/work/SpaceWorkView.vue';
import SpaceWorkCreateView from './components/spaces/space/works/work/SpaceWorkCreateView.vue';
import SpaceWorkEditView from './components/spaces/space/works/work/SpaceWorkEditView.vue';
import SpacePostsView from './components/spaces/space/posts/SpacePostsView.vue';
import SpacePostView from './components/spaces/space/posts/post/SpacePostView.vue';
import SpacePostCreateView from './components/spaces/space/posts/post/SpacePostCreateView.vue';
import SpacePostEditView from './components/spaces/space/posts/post/SpacePostEditView.vue';
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
        path: 'posts/create',
        component: UserPostCreateView,
        name: 'UserPostCreate'
      },
      {
        path: 'posts/:postId/edit',
        component: UserPostEditView,
        name: 'UserPostEdit'
      },
      {
        path: 'posts/:postId',
        component: UserPostView,
        name: 'UserPost'
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
    redirect: { name: 'SpacePosts' },
    children: [
      {
        path: 'posts/create',
        component: SpacePostCreateView,
        name: 'SpacePostCreate'
      },
      {
        path: 'posts/:postId/edit',
        component: SpacePostEditView,
        name: 'SpacePostEdit'
      },
      {
        path: 'posts/:postId',
        component: SpacePostView,
        name: 'SpacePost'
      },
      {
        path: 'posts',
        component: SpacePostsView,
        name: 'SpacePostPosts'
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
    redirect: { name: 'SpaceEditPosts' },
    children: [
      {
        path: 'posts/create',
        component: SpaceEditPostCreateView,
        name: 'SpaceEditPostCreate'
      },
      {
        path: 'posts/:postId/edit',
        component: SpaceEditPostEditView,
        name: 'SpaceEditPostEdit'
      },
      {
        path: 'posts',
        component: SpaceEditPostsView,
        name: 'SpaceEditPosts'
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
