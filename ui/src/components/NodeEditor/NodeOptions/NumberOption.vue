<template>
    <div class="dark-num-input">
        <div @click="decrement" class="__button"><i class="material-icons md-18">keyboard_arrow_left</i></div>
        <div
            v-if="!editMode"
            class="__content"
            @click="enterEditMode"
        >
            <div class="__label .text-truncate">{{ data.Name }}</div>
            <div class="__value">{{ data.Value.toFixed(3) }}</div>
        </div>
        <div v-else class="__content">
            <input
                type="number"
                v-model="tempValue"
                ref="input"
                @blur="leaveEditMode"
            >
        </div>
        <div @click="increment" class="__button"><i class="material-icons md-18">keyboard_arrow_right</i></div>
    </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";

@Component
export default class NumberOption extends Vue {

    @Prop({ type: Object, default: () => {} })
    data!: any;

    editMode = false;
    tempValue = 0;

    increment() {
        this.sendCommand("setValue", this.data.Value + 0.1);
    }

    decrement() {
        this.sendCommand("setValue", this.data.Value - 0.1);
    }

    async enterEditMode() {
        this.tempValue = this.data.Value;
        this.editMode = true;
        await this.$nextTick();
        (this.$refs.input as HTMLElement).focus();
    }

    leaveEditMode() {
        this.sendCommand("setValue", this.tempValue);
        this.editMode = false;
    }

}
</script>