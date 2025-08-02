<template>
  <h1>Hello App!</h1>
  <p>
    <strong>Current route path:</strong> {{ $route.fullPath }}
  </p>
  <nav>
    <RouterLink to="/">Go to Home</RouterLink>
    <RouterLink to="/about">Go to About</RouterLink>
    <RouterLink to="/users/david/posts/post-1">Go to Post 1</RouterLink>
    <RouterLink to="/users/david/posts/post-2">Go to Post 2</RouterLink>
  </nav>
  <main>
    <RouterView />
  </main>
</template>
