<template>
<b-card class="editor" no-body>
    <b-tabs id="scenes" class="h-100" card>

        <template slot="tabs">
            <b-nav-item
                v-for="(scene, id) in scenes"
                :key="id"
                :active="displayed === id"
                @click.prevent="selectScene(id)"
            >{{ scene.Name }}</b-nav-item>
            <b-nav-item
                @click.prevent="addScene"
            >+</b-nav-item>
        </template>

        <div v-if="displayedScene" slot="empty" class="no-body h-100">

            <!-- Toolbar -->
            <b-navbar type="dark" variant="dark" toggleable>
                <b-navbar-toggle target="editor_toolbar_collapse"></b-navbar-toggle>
                <b-collapse is-nav id="editor_toolbar_collapse">
                    <b-navbar-nav>
                        <b-nav-item-dropdown text="Add Node">
                            <template v-for="(nodes, index) in nodeTypes">
                                <b-dropdown-header :key="'dh_' + index">{{ nodeCategories[index] }}</b-dropdown-header>
                                <b-dropdown-item-button
                                    v-for="node in nodes"
                                    :key="node.id"
                                    @click="sendSceneCommand('addNode', node.id)"
                                >{{ node.name }}</b-dropdown-item-button>
                                <b-dropdown-divider v-if="index < nodeCategories.length - 1" :key="'dd_' + index"></b-dropdown-divider>
                            </template>
                        </b-nav-item-dropdown>
                    </b-navbar-nav>
                    <b-navbar-nav class="ml-auto">
                        <b-nav-form>
                            <b-form-input
                                class="mr-2"
                                type="text"
                                placeholder="Scene Name"
                                :value="displayedScene.Name"
                                @input="sendSceneCommand('Name.set', $event)">
                            </b-form-input>
                            <b-button
                                variant="outline-danger" 
                                class="my-2 my-sm-0"
                                @click="sendCommand('delete', displayed)"
                            >Delete Scene</b-button>
                        </b-nav-form>
                    </b-navbar-nav>
                </b-collapse>
            </b-navbar>

            <node-editor
                v-if="displayedScene"
                :scene="displayedScene"
                :rname="displayed"
            ></node-editor>

        </div>

    </b-tabs>
</b-card>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "vue-property-decorator";
import NodeEditor from "../components/NodeEditor/NodeEditor.vue";
import * as _ from "lodash";

@Component({
    components: {
        "node-editor": NodeEditor
    }
})
export default class Editor extends Vue {

    @Prop({ type: Object })
    scenes: any;

    @Prop({ type: String })
    displayed!: string;

    nodeCategories = [ "Input", "Output", "Color", "Converter", "Generator" ];

    get displayedScene() {
        return this.displayed ? this.scenes[this.displayed] : undefined;
    }

    get nodeTypes() {
        if (!this.displayedScene) { return []; }
        return this.nodeCategories.map((name, index) => {
            return _.chain(this.displayedScene.NodeTypes)
                .entries()
                .filter((pair: [string, any]) => pair[1].Category === index)
                .map((pair: [string, any]) => ({ id: pair[0], name: pair[1].Name }))
                .value();
        });
    }

    addScene() {
        this.sendCommand("add");
    }

    selectScene(id: string) {
        this.sendCommand("select", id);
    }

    sendSceneCommand(command: string, payload: any) {
        if (this.displayed) {
            this.sendCommand(`${this.displayed}.${command}`, payload);
        }
    }

}
</script>

<style>
#scenes > .tab-content {
    height: 100%;
}
#scenes > .tab-content > .card-body {
    padding: 0;
    height: 100%;
}

.editor {
    top: 0;
    right: 0;
    bottom: 0;
    left: 0;
    position: absolute !important;
}
</style>
