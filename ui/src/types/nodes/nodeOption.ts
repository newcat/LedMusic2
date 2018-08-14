import { IReactiveObject } from "@/types/reactiveObject";

export enum NodeOptionType {
    NUMBER,
    COLOR,
    BOOL,
    SELECT,
    PREVIEW,
    TEXT,
    CUSTOM
}

export interface INodeOption extends IReactiveObject {
    Name: string;
    Type: NodeOptionType;
}
