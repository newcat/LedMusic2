import Vue from "vue";
import Router from "vue-router";
Vue.use(Router);

export default new Router({
    routes: [
        {
            path: "/editor",
            name: "editor"
        },
        {
            path: "/outputs",
            name: "outputs"
        },
        {
            path: "/",
            redirect: "/editor"
        }
    ],
});
