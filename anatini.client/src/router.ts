import { createWebHistory, createRouter } from 'vue-router'

import AboutView from './components/AboutView.vue';
import HomeView from './components/HomeView.vue';
import LoginView from './components/LoginView.vue';
import ContentCreateView from './components/ContentCreateView.vue';
import ContentEditView from './components/ContentEditView.vue';
import ContentPreviewView from './components/ContentPreviewView.vue';
import ContentView from './components/ContentView.vue';
import AccountView from './components/AccountView.vue';
import SignupFlowView from './components/SignupFlowView.vue';
import UserView from './components/UserView.vue';
import ChannelCreateView from './components/ChannelCreateView.vue';
import ChannelEditView from './components/ChannelEditView.vue';
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
    path: '/channels/:channelId/contents/create',
    component: ContentCreateView,
    name: 'ContentCreate',
  },
  {
    path: '/channels/:channelId/contents/:contentId/edit',
    component: ContentEditView,
    name: 'ContentEdit',
  },
  {
    path: '/channels/:channelId/contents/:contentId/preview',
    component: ContentPreviewView,
    name: 'ContentPreview',
  },
  {
    path: '/channels/:channelId/contents/:contentId',
    component: ContentView,
    name: 'Content',
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
