<template>
    <div id="app">
        
        <!--  style="flex: 0 1 auto;" -->
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

        <b-container v-if="connecting" class="mt-3">
            <b-alert show>Connecting...</b-alert>
        </b-container>

        <!--  style="flex: 1 1 auto;" -->
        <b-container fluid style="flex: 1 1 auto;" class="mt-3" v-else>
                
                <editor
                    v-show="$route.name === 'editor'"
                    :scenes="state.Scenes"
                    :displayed="state.DisplayedSceneId"
                    rname="Scenes"
                ></editor>

        </b-container>

    </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import Editor from "./views/Editor.vue";

import apply from "./stateApplier";

@Component({
    components: {
        "editor": Editor
    }
})
export default class App extends Vue {

    ws!: WebSocket;

    connecting = true;
    state: any = {};

    mounted() {
        this.ws = new WebSocket("ws://localhost:48235");
        this.ws.onopen = () => { this.connecting = false; };
        this.ws.onmessage = this.handleMessage;
    }

    sendCommand(command: string, payload: any) {
        if (this.ws && this.ws.readyState === this.ws.OPEN) {
            this.ws.send(JSON.stringify({
                type: "command",
                command,
                payload
            }));
        }
    }

    handleMessage(ev: MessageEvent) {

        const msg = JSON.parse(ev.data);
        console.log(msg);

        switch (msg.type) {
            case "state":
                console.log("Received state update");
                apply(msg.state, this.state);
                break;
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
