import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { UserService } from '../../../../core/services/user.service';
import { UserInEvent } from '../../../../core/model/response/participant.model';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { RippleModule } from 'primeng/ripple';
import { TagModule } from 'primeng/tag';
import { TooltipModule } from 'primeng/tooltip';
import { VndCurrencyPipe } from '../../../../shared/pipes/vnd-currency.pipe';
import { ImageUrlPipe } from '../../../../shared/pipes/image-url.pipe';

@Component({
  selector: 'app-participant-detail',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    TableModule,
    ButtonModule,
    RippleModule,
    TagModule,
    TooltipModule,
    VndCurrencyPipe,
    ImageUrlPipe
  ],
  templateUrl: './participant-detail.component.html',
  styleUrl: './participant-detail.component.scss'
})
export class ParticipantDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private userService = inject(UserService);

  participant: UserInEvent | null = null;
  loading = true;
  totalSpent = 0;

  ngOnInit(): void {
    const userId = this.route.snapshot.paramMap.get('id');
    if (userId) {
      this.loadParticipantDetail(userId);
    }
  }

  loadParticipantDetail(userId: string) {
    this.loading = true;
    this.userService.GetParticipantDetail(userId).subscribe({
      next: (res) => {
        this.participant = res;
        this.calculateTotalSpent();
        this.loading = false;
      },
      error: (err) => {
        console.error('Error fetching participant detail', err);
        this.loading = false;
      }
    });
  }

  calculateTotalSpent() {
    this.totalSpent = 0;
    if (this.participant && this.participant.Events) {
      for (const event of this.participant.Events) {
        if (event.Tickets) {
          for (const ticket of event.Tickets) {
            this.totalSpent += ticket.Price * ticket.Quantity;
          }
        }
      }
    }
  }
}
