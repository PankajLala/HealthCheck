import { inject, TestBed, tick, fakeAsync } from "@angular/core/testing";
import { HubConnectionBuilder } from "@microsoft/signalr";
import { SignalRService } from './signal-r.service';


class HubConnectionMock {
  private methods: Map<string, (...args: any[]) => void> = new Map<
    string,
    (...args: any[]) => void
    >();

  on(methodName: string, newMethod: (...args: any[]) => void): void {
    this.methods.set(methodName, newMethod);
  }

  async start() {
    return Promise.resolve();
  }

  stop() {}

  invoke(methodName, event) {
    const func = this.methods.get(methodName);
    if (func) {
      func(event);
    }
  }

  onclose(callback: (error?: Error) => void) {
    this.on("close", callback);
  }
}

describe('SignalRService', () => {

  let hubConnectionBuilderMock, hubConnection: HubConnectionMock;

  beforeEach(() => {
    hubConnection = new HubConnectionMock();

    hubConnectionBuilderMock = jasmine.createSpyObj("hubConnectionBuilder", {
      withUrl: () => {},
      build: hubConnection,
    });

    hubConnectionBuilderMock.withUrl.and.returnValue(hubConnectionBuilderMock);

    TestBed.configureTestingModule({
      providers: [
        SignalRService,
        { provide: HubConnectionBuilder, useValue: hubConnectionBuilderMock },
      ],
    });
  })


  it("should be created", inject(
    [SignalRService],
    (service: SignalRService) => {
      expect(service).toBeTruthy();
    }
  ));

});

