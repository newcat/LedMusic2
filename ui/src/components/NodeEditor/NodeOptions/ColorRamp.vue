<template>
    <div>
        <preview-option></preview-option>
        <svg>
            <g>
                <path
                    v-for="(marker, key) in markers"
                    :key="key"
                    d="M2 5 V 1 L1 0 L0 1 V 5 H 2"
                    :transform="'translateX(' + marker.Position + ')'"
                    :stroke="marker.Selected ? 'cyan' : 'black'"
                    :fill="toHex(marker.Color)"
                ></path>
            </g>
        </svg>
    </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import PreviewOption from "./PreviewOption.vue";
import { fromHex, toHex } from "@/ledColor";

@Component({
    components: {
        "preview-option": PreviewOption
    }
})
export default class ColorRamp extends Vue {

    toHex = toHex;

}
</script>
