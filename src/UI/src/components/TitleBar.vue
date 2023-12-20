<script setup lang="ts">

</script>

<template>
  <div data-tauri-drag-region class="title-bar">
    <div></div>
    <div class="title-bar-button-box">
      <div class="title-bar-button">
        <img src="https://api.iconify.design/mdi:window-minimize.svg" alt="minimize"/>
      </div>
      <div class="title-bar-button">
        <img src="https://api.iconify.design/mdi:window-maximize.svg" alt="maximize"/>
      </div>
      <div class="title-bar-button">
        <img src="https://api.iconify.design/mdi:close.svg" alt="close"/>
      </div>
    </div>
  </div>
</template>

<style scoped>
.title-bar {
  height: 32px;
  user-select: none;
  display: flex;
  justify-content: space-between;
  align-items: center;
  top: 0;
  left: 0;
  right: 0;
  z-index: 999;
}

.title-bar-button-box {
  display: flex;
  flex-direction: row;
  gap: 2px;
  margin-right: 6px;
}

.title-bar-button {
  display: inline-flex;
  justify-content: center;
  align-items: center;
  width: 30px;
  height: 30px;
}

.title-bar-button:hover {
  background-color: var(--accent-color);
}
</style>
