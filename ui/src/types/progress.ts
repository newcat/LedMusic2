import { IReactiveObject } from "@/types/reactiveObject";

export interface IProgress extends IReactiveObject {
    Description: string;
    Progress: number;
}
