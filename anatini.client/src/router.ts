import { createWebHistory, createRouter } from 'vue-router'

import HomeView from './components/HomeView.vue'
import AboutView from './components/AboutView.vue'
import PostView from './components/PostView.vue'

const routes = [
  { path: '/', component: HomeView },
  { path: '/about', component: AboutView },
  { path: '/users/:userHandle/posts/:postHandle', component: PostView },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router
