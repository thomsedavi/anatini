import { createWebHistory, createRouter } from 'vue-router'

import AboutView from './components/AboutView.vue';
import HomeView from './components/HomeView.vue';
import LoginView from './components/LoginView.vue';
import PostView from './components/PostView.vue';
import AccountView from './components/AccountView.vue';
import SignupFlowView from './components/SignupFlowView.vue';
import UserView from './components/UserView.vue';
import ChannelEditView from './components/ChannelEditView.vue';
import ChannelView from './components/ChannelView.vue';
import UsersView from './components/UsersView.vue';

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
    path: '/channels/:channelSlug/posts/:postSlug',
    component: PostView,
  },
  {
    path: '/users/:userSlug',
    component: UserView,
  },
  {
    path: '/channels/:channelSlug/edit',
    component: ChannelEditView,
  },
  {
    path: '/channels/:channelSlug',
    component: ChannelView,
  },
  {
    path: '/users',
    component: UsersView,
  },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router
