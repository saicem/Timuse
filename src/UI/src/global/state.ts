import {reactive} from "vue"

export const themeStore = reactive({
  isLight: true,
  light() {
    this.isLight = true
  },
  dark() {
    this.isLight = false
  }
})

export const tabStore = reactive({
  cur: "home",
  change(next: string) {
    this.cur = next
  }
})
