import { IScene } from "@/types/scene";
import { IProgress } from "@/types/progress";
import { IOutputManager } from "@/types/outputs/outputManager";

export interface IMain {
    ActiveSceneId: string;
    DisplayedSceneId: string;
    IsRunning: boolean;
    Scenes: Record<string, IScene>;
    Progress: Record<string, IProgress>;
    OutputManager: IOutputManager;
}
