using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using JetBrains.Annotations;

using Volo.Abp;

namespace Vsky.WorkflowEngine.WorkflowEngineSettingsManages
{
    public class WorkflowEngineSettingsManage : FullAuditedAggregateRoot<Guid>
    {
        [CanBeNull]
        public virtual string? ProjectName { get; set; }

        [CanBeNull]
        public virtual string? Description { get; set; }

        public virtual int HistorySaveDays { get; set; }

        [CanBeNull]
        public virtual string? DBInfoDescription { get; set; }

        public WorkflowEngineSettingsManage()
        {

        }

        public WorkflowEngineSettingsManage(Guid id, string projectName, string description, int historySaveDays, string dBInfoDescription)
        {

            Id = id;
            if (historySaveDays < WorkflowEngineSettingsManageConsts.HistorySaveDaysMinLength)
            {
                throw new ArgumentOutOfRangeException(nameof(historySaveDays), historySaveDays, "The value of 'historySaveDays' cannot be lower than " + WorkflowEngineSettingsManageConsts.HistorySaveDaysMinLength);
            }

            if (historySaveDays > WorkflowEngineSettingsManageConsts.HistorySaveDaysMaxLength)
            {
                throw new ArgumentOutOfRangeException(nameof(historySaveDays), historySaveDays, "The value of 'historySaveDays' cannot be greater than " + WorkflowEngineSettingsManageConsts.HistorySaveDaysMaxLength);
            }

            ProjectName = projectName;
            Description = description;
            HistorySaveDays = historySaveDays;
            DBInfoDescription = dBInfoDescription;
        }

    }
}