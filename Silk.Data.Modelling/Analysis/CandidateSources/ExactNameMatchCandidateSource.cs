using System;
using System.Collections.Generic;

namespace Silk.Data.Modelling.Analysis.CandidateSources
{
	public class ExactNameMatchCandidateSource : IIntersectCandidateSource
	{
		public IEnumerable<IntersectCandidate> GetIntersectCandidates(IModel leftModel, IModel rightModel)
		{
			throw new NotImplementedException();
		}
	}
}
