import { Component, computed, inject, signal } from '@angular/core';
import { CompanyStore } from '../../../store/company.store';
import { ActivatedRoute } from '@angular/router';
import { DatePipe } from '@angular/common';
import { Badge } from "../../../../../shared/ui/badge/badge";
import { ApplicationStatus } from '../../../../../shared/models/application-status';
import { BadgeVariant } from '../../../../../shared/models/badge-variant';
import { Message } from '../../../../../shared/ui/message/message';

@Component({
  selector: 'app-application-page',
  imports: [
    DatePipe,
    Badge,
    Message
  ],
  templateUrl:
    './application-page.html',
  styleUrl: './application-page.scss',
})
export class ApplicationPage {

  // DI
  private readonly companyStore = inject(CompanyStore);
  private readonly route = inject(ActivatedRoute);
  readonly applications = this.companyStore.applications;
  private id!: number;

  request = this.companyStore.request

  ngOnInit(): void {
    this.request.set({
      state: 'idle',
      message: ''
    })

    const id = Number(
      this.route.snapshot.paramMap.get('id')
    )

    if (Number.isNaN(id))
      return;

    this.id = id;
    this.companyStore.getContractApplications(id)
      .subscribe()
    console.log(this.applications())
  }

  getVariant(status: ApplicationStatus) {
    return this.STATUS_VARIANTS[status]
  }

  accept(applicationId: number, contractId: number) {
    this.companyStore.acceptApplication(applicationId, contractId)
      .subscribe()
  }
  reject(applicationId: number) {
    this.companyStore.rejectApplication(applicationId)
      .subscribe()
  }

  STATUS_VARIANTS: Record<ApplicationStatus, BadgeVariant> = {
    Accepted: 'success',
    Pending: 'pending',
    Rejected: 'rejected'
  };

}
