<template>
<div
    :class="['dark-select', { '--open': open }]"
    @click="open = !open"
    v-click-outside="() => { open = false; }"
>
    <div class="__selected">
        <div class="__text">{{ selectedName }}</div>
        <div class="__icon"><i class="material-icons md-18">keyboard_arrow_down</i></div>
    </div>
    <transition name="slide-fade">
        <div class="__dropdown" v-show="open">
            <div class="item --header">{{ data.Name }}</div>
            <div
                v-for="(value, key) in data.Options"
                :key="key"
                :class="['item', { '--active': data.SelectedId === key }]"
                @click="sendCommand('setValue', key)"
            >
                {{ data.ItemDisplayPropertyName ? value[data.ItemDisplayPropertyName] : value }}
            </div>
        </div>
    </transition>
</div>
</template>

<script lang="ts">
import { Component, Vue, Prop } from "vue-property-decorator";

@Component
export default class SelectOption extends Vue {

    open = false;

    @Prop({ type: Object, default: () => {} })
    data!: any;

    get selectedName() {
        if (this.data.Options[this.data.SelectedId]) {
            const value = this.data.Options[this.data.SelectedId];
            return this.data.ItemDisplayPropertyName ? value[this.data.ItemDisplayPropertyName] : value;
        } else {
            return "";
        }
    }

}
</script>

<style lang="scss" scoped>

</style>
