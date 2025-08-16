import { createWebHistory, createRouter, type RouteLocationNormalizedGeneric } from 'vue-router'

import AboutView from './components/AboutView.vue';
import HomeView from './components/HomeView.vue';
import LoginView from './components/LoginView.vue';
import PostView from './components/PostView.vue';
import SettingsView from './components/SettingsView.vue';
import SignupView from './components/SignupView.vue';
import UserView from './components/UserView.vue';
import UsersView from './components/UsersView.vue';
import { store } from './store.ts'

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
    component: SignupView,
  },
  {
    path: '/login',
    component: LoginView,
  },
  {
    path: '/settings',
    component: SettingsView,
    meta: { requiresAuth: true },
  },
  {
    path: '/users/:userHandle/posts/:postHandle',
    component: PostView,
  },
  {
    path: '/users/:userHandle',
    component: UserView,
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

router.beforeEach((to: RouteLocationNormalizedGeneric) => {
  if (to.meta.requiresAuth && !store.isLoggedIn) {
    return {
      path: '/login',
      query: { redirect: to.fullPath },
    }
  }
});

export default router
