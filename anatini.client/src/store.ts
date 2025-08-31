import { reactive } from 'vue'
import { type Router } from 'vue-router';

export const store = reactive({
  isLoggedIn: localStorage.getItem('jwtToken') !== null,
  jwtToken: localStorage.getItem('jwtToken'),
  logIn(token: string): void {
    this.jwtToken = token;
    this.isLoggedIn = true;
    localStorage.setItem("jwtToken", token);
  },
  logOut(router: Router): void {
    this.jwtToken = null;
    this.isLoggedIn = false;
    localStorage.removeItem('jwtToken');
    router.replace({ path: '/' });
  },
});
