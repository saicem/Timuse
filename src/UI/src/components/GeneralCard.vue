<script setup lang="ts">
const props = defineProps<{
  icon: string,
  value: string,
  unit: string,
  illustration: string,
}>()

</script>

<template>
  <div class="container root">
    <div class="icon">
      <img :src="props.icon" alt="card kind"/>
    </div>
    <div class="container" style="align-items: start;">
      <div class="vice-info">{{ props.illustration }}</div>
      <div>
        <span class="value">{{ props.value }}</span>
        <span>{{ props.unit }}</span>
      </div>
    </div>
  </div>
</template>

<style scoped>

.root {
  flex-direction: row;
  justify-content: start;
  padding: 16px 24px;
  color: rgb(128, 128, 128);
  width: 100%;
  gap: 16px;
}

.value {
  color: darkslategray;
  font-size: x-large;
  margin-right: 6px;
}

.vice-info {
  margin-bottom: 4px;
}

</style>
