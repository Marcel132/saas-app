import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DatePipe } from '@angular/common';
import { Badge } from "../../../../../shared/ui/badge/badge";
import { ApplicationStatus } from '../../../../../shared/models/application-status';
import { BadgeVariant } from '../../../../../shared/models/badge-variant';
import { Message } from '../../../../../shared/ui/message/message';
import { CompanyApplicationStore } from '../../../store/company-application.store';

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
  private readonly companyApplicationStore = inject(CompanyApplicationStore);
  private readonly route = inject(ActivatedRoute);
  readonly applications = this.companyApplicationStore.applications;
  private id!: number;

  request = this.companyApplicationStore.applicationRequest;

  ngOnInit(): void {
    this.companyApplicationStore.clearRequestState();

    const id = Number(
      this.route.snapshot.paramMap.get('id')
    )

    if (Number.isNaN(id))
      return;

    this.id = id;
    this.companyApplicationStore.getContractApplications(id)
      .subscribe()
  }

  getVariant(status: ApplicationStatus) {
    return this.STATUS_VARIANTS[status]
  }

  accept(applicationId: number, contractId: number) {
    this.companyApplicationStore.acceptApplication(applicationId, contractId)
      .subscribe()
  }
  reject(applicationId: number) {
    this.companyApplicationStore.rejectApplication(applicationId)
      .subscribe()
  }

  STATUS_VARIANTS: Record<ApplicationStatus, BadgeVariant> = {
    Accepted: 'success',
    Pending: 'pending',
    Rejected: 'rejected'
  };

}
