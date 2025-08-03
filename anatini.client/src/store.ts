import { reactive } from 'vue'

export const store = reactive({
  isLoggedIn: localStorage.getItem('jwtToken') !== null,
  logIn(token: string) {
    localStorage.setItem("jwtToken", token)
    this.isLoggedIn = true;
  },
  logOut() {
    localStorage.removeItem('jwtToken');
    this.isLoggedIn = false;
  },
});
