import { Component, NgZone, OnInit } from '@angular/core';
import { SignalRService } from './services/signal-r.service';
import { ServerStatus } from './interface/ServerStatus';
import { AppComponentService } from './services/app.component.service';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  public title = 'XYZ Co Dashboard';
  public serverStatuses: ServerStatus[] = [];
  public serverStatusesHistory: ServerStatus[] = [];

  constructor(public signalRService: SignalRService, protected appComponentService: AppComponentService, private _ngZone: NgZone) {}

  ngOnInit() {
    this.signalRService.startConnection();
    this.signalRService.serverStatusDataListner();
    this.getHistoricalServerStatus();

    this.signalRService._responseReceivedEvents$.subscribe((data:ServerStatus[]) => {
      if(data){
        this.serverStatuses.length=0;
          this._ngZone.run(()=>{
            this.serverStatuses = data;
          })
      }
    })
  }

  getHistoricalServerStatus(){
    this.appComponentService.getServerStatuses()
    .subscribe((response: ServerStatus[]) => {
      this.serverStatusesHistory = response;
    })
  }

  GetServerStatus() {
    this.getHistoricalServerStatus();
  };

}
