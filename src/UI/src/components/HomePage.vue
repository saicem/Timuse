<script setup lang="ts">

import app from "../assets/app.svg";
import usage from "../assets/usage.svg";
import up from "../assets/up.svg";
import GeneralCard from "./GeneralCard.vue";
import DayOfWeekChart from "./DayOfWeekChart.vue";
</script>

<template>
  <div
    class="container"
    style="gap: 16px;padding: 8px"
  >
    <div class="cards">
      <GeneralCard
        class="card"
        :icon="app"
        value="16"
        unit="个"
        illustration="应用量"
      />
      <GeneralCard
        class="card"
        :icon="usage"
        value="3.89"
        unit="小时"
        illustration="总计"
      />
      <GeneralCard
        class="card"
        :icon="up"
        value="2.13"
        unit="小时"
        illustration="最常使用"
      />
    </div>
    <DayOfWeekChart
      class="card"
      style="aspect-ratio: 2; flex-shrink: 0; flex-grow: 1;"
    />
  </div>
</template>

<style scoped>
.cards {
  display: flex;
  flex-direction: row;
  justify-content: space-between;
  gap: 16px;
}
</style>