<template>
    <div id="app">
        
        <!--  style="flex: 0 1 auto;" -->
        <b-navbar style="flex: 0 1 auto;" type="dark" variant="primary" toggleable="md">

            <b-navbar-brand>LedMusic2</b-navbar-brand>
            <b-navbar-toggle target="nav_collapse"></b-navbar-toggle>
            
            <b-collapse is-nav id="nav_collapse">
                <b-navbar-nav>
                    <b-nav-item to="editor">Editor</b-nav-item>
                    <b-nav-item to="outputs">Outputs</b-nav-item>
                    <b-nav-item>VST Channels</b-nav-item>
                    <b-nav-item @click="sendLoadCommand">Load</b-nav-item>
                    <b-nav-item @click="sendSaveCommand">Save</b-nav-item>
                </b-navbar-nav>
            </b-collapse>

        </b-navbar>

        <b-container v-if="connecting" class="mt-3">
            <b-alert show>Connecting...</b-alert>
        </b-container>

        <!--  style="flex: 1 1 auto;" -->
        <div v-else style="flex: 1 1 auto; position: relative;">
                
                <editor
                    v-show="$route.name === 'editor'"
                    :scenes="state.Scenes"
                    :displayed="state.DisplayedSceneId"
                    rname="Scenes"
                ></editor>

                <output-manager
                    v-show="$route.name === 'outputs'"
                    :state="state.OutputManager"
                    rname="OutputManager"
                ></output-manager>

        </div>

    </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import Editor from "./views/Editor.vue";
import OutputManager from "./views/OutputManager.vue";

import apply from "./stateApplier";
import { setTimeout } from "timers";

@Component({
    components: {
        "editor": Editor,
        "output-manager": OutputManager
    }
})
export default class App extends Vue {

    ws!: WebSocket;

    connecting = true;
    state: any = {};

    mounted() {
        this.connect();
    }

    connect() {
        this.connecting = true;
        this.ws = new WebSocket("ws://localhost:48235");
        this.ws.onopen = () => { this.connecting = false; };
        this.ws.onclose = () => this.connect;
        this.ws.onmessage = this.handleMessage;
        this.ws.onerror = (err) => {
            console.log(err);
            setTimeout(this.connect, 1000);
        };
    }

    sendRaw(data: any) {
        if (this.ws && this.ws.readyState === this.ws.OPEN) {
            this.ws.send(JSON.stringify(data));
        }
    }

    sendCommand(command: string, payload: any) {
        this.sendRaw({
            type: "command",
            command,
            payload
        });
    }

    sendLoadCommand() {
        const path = prompt("Project Name");
        this.sendRaw({ type: "load", path });
    }

    sendSaveCommand() {
        const path = prompt("Project Name");
        this.sendRaw({ type: "save", path });
    }

    handleMessage(ev: MessageEvent) {

        const msg = JSON.parse(ev.data);
        console.log(msg);

        switch (msg.type) {
            case "fullstate":
                this.state = {};
                apply(msg.state, this.state);
                break;
            case "stateupdate":
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
