import { createWebHistory, createRouter } from 'vue-router'

import HomeView from './components/HomeView.vue'
import AboutView from './components/AboutView.vue'
import PostView from './components/PostView.vue'
import SignupView from './components/SignupView.vue'
import UserView from './components/UserView.vue'

const routes = [
  { path: '/', component: HomeView },
  { path: '/about', component: AboutView },
  { path: '/signup', component: SignupView },
  { path: '/users/:userHandle/posts/:postHandle', component: PostView },
  { path: '/users/:userHandle', component: UserView },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router
