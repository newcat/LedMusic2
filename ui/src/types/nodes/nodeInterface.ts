import { ConnectionType } from "@/types/connections/connectionType";
import { INodeOption } from "@/types/nodes/nodeOption";

export interface INodeInterface {
    Name: string;
    ConnectionType: ConnectionType;
    IsInput: boolean;
    IsConnected: boolean;
    Option: INodeOption;
}
