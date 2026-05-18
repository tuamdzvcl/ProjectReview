import { TicketInfo } from './../core/model/response/participant.model';
import { EventService } from './../core/services/event.service';
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EventRequest } from '../core/model/request/eventRequest.model';
import { EventModel } from '../core/model/response/event.model';
import { FormsModule } from '@angular/forms';
import { ApiErrorHandler } from '../core/utils/api-error-handler.util';

@Component({
  selector: 'app-test',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './test.component.html',
  styleUrl: './test.component.scss',
})
export class TestComponent implements OnInit {
  eventData: EventModel[] = [];
  isLoaing: boolean = true;
  selectedEvent: EventModel | null = null;
  isPopUpOpen: boolean = false;

  popUpMod: 'view' | 'add' | 'edit' = 'view';
  eventForm: any = { Title: '', Location: '' };
  id: string = '';

  constructor(private eventService: EventService) {}

  ngOnInit(): void {
    this.loadEvent();
  }
  loadEvent() {
    this.eventService.GetEvents(1, 10, '').subscribe({
      next: (res) => {
        this.eventData = res.items;
        this.isLoaing = false;
        console.log('dữ liệu call api', this.eventData);
      },
      error: (err) => {
        console.log(err);
        this.isLoaing = false;
      },
    });
  }

  OpenPopUp(event: EventModel) {
    this.popUpMod = 'view';
    this.selectedEvent = event;
    this.isPopUpOpen = true;
  }
  OpenAddPopUp() {
    this.popUpMod = 'add';
    this.eventForm = { Title: '', Location: '' };
    this.isPopUpOpen = true;
  }
  ClosePopUp() {
    this.isPopUpOpen = false;
    this.selectedEvent = null;
  }
  OpenEditPopup(event: EventModel) {
    this.selectedEvent = event;
    this.popUpMod = 'edit';
    this.eventForm = { ...event };
    this.isPopUpOpen = true;
  }

  SaveEvent() {
    const formData = new FormData();
    formData.append('Title', this.eventForm.Title);
    formData.append('Location', this.eventForm.Location);

    if (this.popUpMod === 'add') {
      this.eventService.CreateEvent(formData).subscribe({
        next: (res) => {
          alert('thêm thành công');
          this.loadEvent();
          this.ClosePopUp();
          console.log(res);
        },
        error: (err) => {
          console.log(err);
        },
      });
    } else if (this.popUpMod === 'edit' && this.selectedEvent?.EventID) {
      console.log(this.selectedEvent.EventID);

      this.eventService
        .UpdateEvent(this.selectedEvent?.EventID.toString(), formData)
        .subscribe({
          next: (res) => {
            alert('sửa thành công');
            this.loadEvent();
            this.ClosePopUp();
          },
          error: (err) => {
            ApiErrorHandler.handleError(err);
            console.log();
          },
        });
    }
  }
}
