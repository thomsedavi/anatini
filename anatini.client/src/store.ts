import { reactive } from 'vue'
import { type Router } from 'vue-router';

export const store = reactive({
  isLoggedIn: localStorage.getItem('jwtToken') !== null,
  jwtToken: localStorage.getItem('jwtToken'),
  logIn(token: string): void {
    localStorage.setItem("jwtToken", token)
    this.isLoggedIn = true;
  },
  logOut(router: Router): void {
    localStorage.removeItem('jwtToken');
    this.isLoggedIn = false;
    router.replace({ path: '/' });
  },
});
