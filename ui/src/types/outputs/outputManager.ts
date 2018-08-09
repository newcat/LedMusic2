import { IOutput } from "@/types/outputs/output";
import { IOutputType } from "@/types/outputs/outputType";

export interface IOutputManager {
    Outputs: Record<string, IOutput>;
    OutputTypes: Record<string, IOutputType>;
}
