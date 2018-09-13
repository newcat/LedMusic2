import Vue from "vue";
import App from "@/App.vue";
import router from "@/router";
import store from "@/store";

import "bootstrap/dist/css/bootstrap.css";
import "bootstrap-vue/dist/bootstrap-vue.css";
import BootstrapVue from "bootstrap-vue";
Vue.use(BootstrapVue);

import "./styles/styles.scss";

import commandMixin from "@/commandMixin";
Vue.mixin(commandMixin);

// @ts-ignore
import vClickOutside from "v-click-outside";
Vue.directive("click-outside", vClickOutside.directive);

import "./components/Dark/index";

Vue.config.productionTip = false;
new Vue({
    router,
    store,
    render: (h) => h(App),
}).$mount("#app");
