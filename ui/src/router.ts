import Vue from "vue";
import Router from "vue-router";
Vue.use(Router);

import Editor from "./views/Editor.vue";

export default new Router({
    routes: [
        {
            path: "/editor",
            name: "editor",
            component: Editor
        },
        {
            path: "/",
            redirect: "/editor"
        }
    ],
});
