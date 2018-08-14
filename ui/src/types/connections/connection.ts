import { IReactiveObject } from "@/types/reactiveObject";

export interface IConnection extends IReactiveObject {
    InputNodeId: string;
    InputInterfaceId: string;
    OutputNodeId: string;
    OutputInterfaceId: string;
}
