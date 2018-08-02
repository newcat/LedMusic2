<template>
    <div id="app">
        
        <b-navbar style="flex: 0 1 auto;" type="dark" variant="primary" toggleable="md">

            <b-navbar-brand>LedMusic2</b-navbar-brand>
            <b-navbar-toggle target="nav_collapse"></b-navbar-toggle>
            
            <b-collapse is-nav id="nav_collapse">
                <b-navbar-nav>
                    <b-nav-item to="editor">Editor</b-nav-item>
                    <b-nav-item>Outputs</b-nav-item>
                    <b-nav-item>VST Channels</b-nav-item>
                </b-navbar-nav>
            </b-collapse>

        </b-navbar>

        <b-container class="mt-3">
            <b-alert show>Connecting...</b-alert>
        </b-container>

        <!--<router-view style="flex: 1 1 auto;"></router-view>-->
        <node-editor v-if="selectedScene" style="flex: 1 1 auto;" :scene="selectedScene"></node-editor>

    </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import NodeEditor from "./views/NodeEditor.vue";

import apply from "./stateApplier";

@Component({
    components: {
        "node-editor": NodeEditor
    }
})
export default class App extends Vue {

    ws!: WebSocket;

    connecting = true;
    state: any = {};

    get selectedScene() {
        if (this.state && this.state.Scenes) {
            return this.state.Scenes[Object.keys(this.state.Scenes)[0]];
        } else {
            return undefined;
        }
    }

    mounted() {
        this.ws = new WebSocket("ws://localhost:48235");
        this.ws.onopen = () => { this.connecting = false; };
        this.ws.onmessage = this.handleMessage;
    }

    handleMessage(ev: MessageEvent) {

        const msg = JSON.parse(ev.data);

        switch (msg.type) {
            case "state":
                apply(msg.state, this.state);
            default:
                console.log("Unknown message type", msg.type);
        }

    }

}
</script>


<style lang="scss">
html, body {
    height: 100%;
    margin: 0;
}

#app {
    display: flex;
    flex-flow: column;
    height: 100%;
}
</style>
