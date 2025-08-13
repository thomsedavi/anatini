import { createWebHistory, createRouter } from 'vue-router'

import HomeView from './components/HomeView.vue';
import AboutView from './components/AboutView.vue';
import PostView from './components/PostView.vue';
import SignupView from './components/SignupView.vue';
import VerifyEmailView from './components/VerifyEmailView.vue';
import LoginView from './components/LoginView.vue';
import UserView from './components/UserView.vue';

const routes = [
  { path: '/', component: HomeView },
  { path: '/about', component: AboutView },
  { path: '/signup', component: SignupView },
  { path: '/verifyEmail', component: VerifyEmailView },
  { path: '/login', component: LoginView },
  { path: '/users/:userHandle/posts/:postHandle', component: PostView },
  { path: '/users/:userHandle', component: UserView },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router
