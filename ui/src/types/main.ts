import { IScene } from "@/types/scene";
import { IProgress } from "@/types/progress";
import { IOutputManager } from "@/types/outputs/outputManager";
import { IReactiveObject } from "@/types/reactiveObject";

export interface IMain extends IReactiveObject {
    ActiveSceneId: string;
    DisplayedSceneId: string;
    IsRunning: boolean;
    Scenes: Record<string, IScene>;
    Progress: Record<string, IProgress>;
    OutputManager: IOutputManager;
}
