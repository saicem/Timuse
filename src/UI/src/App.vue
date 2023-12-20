<script setup lang="ts">
import TitleBar from "./components/TitleBar.vue"
import TabBar from "./components/TabBar.vue"
import HomePage from "./components/HomePage.vue"


</script>

<template>
  <div class="container" style="background-color: var(--app-bg-color);">
    <TitleBar/>
    <div class="frame">
      <TabBar/>
      <HomePage style="flex: 1 0 600px;width: 600px"/>
    </div>
  </div>
</template>

<style scoped>

.frame {
  display: flex;
}

</style>
