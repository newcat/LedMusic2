import { IOutput } from "@/types/outputs/output";
import { IOutputType } from "@/types/outputs/outputType";
import { IReactiveObject } from "@/types/reactiveObject";

export interface IOutputManager extends IReactiveObject {
    Outputs: Record<string, IOutput>;
    OutputTypes: Record<string, IOutputType>;
}
