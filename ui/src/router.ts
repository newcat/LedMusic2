import Vue from "vue";
import Router from "vue-router";
Vue.use(Router);

import NodeEditor from "./views/NodeEditor.vue";

export default new Router({
    routes: [
        {
            path: "/editor",
            name: "editor",
            component: NodeEditor
        },
        {
            path: "/",
            redirect: "/editor"
        }
    ],
});
