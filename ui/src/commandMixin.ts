import Vue from "vue";

export default Vue.extend({
    props: {
        rname: String
    },
    methods: {
        sendCommand(command: string, payload?: any) {
            const prefix = this.rname ? `${this.rname}.` : "";
            this.$parent.sendCommand(prefix + command, payload);
        }
    }
});
